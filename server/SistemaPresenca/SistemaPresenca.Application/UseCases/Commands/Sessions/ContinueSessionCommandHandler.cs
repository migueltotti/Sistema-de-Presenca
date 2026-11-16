using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed class ContinueSessionCommandHandler(
    ISessionRepository sessionRepository,
    IRoomRepository roomRepository,
    IRoomBookingRepository roomBookingRepository,
    IUserRepository userRepository,
    IMqttProducer mqttProducer,
    ILogger<ContinueSessionCommandHandler> logger) : ICommandHandler<ContinueSessionCommand, Result>
{
    public async Task<Result> HandleAsync(ContinueSessionCommand command, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.GetOneAsync(
            x => x.Id == command.SessionId,
            includes: ["Subject.Professors", "Room"],
            cancellationToken: cancellationToken);

        if (session is null)
        {
            logger.LogError("Session with id {SessionId} - not found", command.SessionId);
            return Result.Failure(SessionErrors.NotFound);
        }

        if (session.FinalizedAt is not null)
        {
            logger.LogError("Session with id {SessionId} is already finished", command.SessionId);
            return Result.Failure(SessionErrors.AlreadyFinished);
        }

        var user = await userRepository.GetOneAsync(
            x => x.TagId == command.ProfessorTagId && (x.Role == UserRole.Professor || x.Role == UserRole.Admin),
            cancellationToken: cancellationToken);

        if (user is null)
        {
            logger.LogError("Professor or Admin with tagId {ProfessorTagId} - not found", command.ProfessorTagId);
            return Result.Failure(UserErrors.ProfessorNotFound);
        }

        if (user.Role == UserRole.Professor && !session.Subject.Professors.Any(p => p.Id == user.Id))
        {
            logger.LogError("Professor with tagId {ProfessorTagId} is not assigned to session {SessionId}", command.ProfessorTagId, command.SessionId);
            return Result.Failure(SessionErrors.ProfessorMismatch);
        }

        var targetRoomId = command.RoomId ?? session.RoomId;
        if (!targetRoomId.HasValue)
        {
            logger.LogError("Session {SessionId} has no associated room to start attendance.", command.SessionId);
            return Result.Failure(new Error("Session.RoomRequired", "É necessário registrar uma sala para iniciar a aula."));
        }

        var room = session.Room is not null && session.Room.Id == targetRoomId.Value
            ? session.Room
            : await roomRepository.GetOneAsync(r => r.Id == targetRoomId.Value, cancellationToken: cancellationToken);

        if (room is null)
        {
            logger.LogError("Room with id {RoomId} - not found", targetRoomId.Value);
            return Result.Failure(RoomErrors.NotFound);
        }

        if (room.Status != RoomStatus.Active)
        {
            logger.LogError("Room with id {RoomId} is inactive.", room.Id);
            return Result.Failure(RoomAccessPermissionErrors.RoomInactive);
        }

        var now = DateTime.UtcNow;
        var estimatedEnd = now.AddMinutes(session.TotalLessons * 50);
        var upcomingBookings = await roomBookingRepository.GetUpcomingByRoomIdAsync(room.Id, now, cancellationToken);

        var hasConflict = upcomingBookings.Any(b =>
            b.Id != session.RoomBookingId &&
            b.ProfessorId != user.Id &&
            b.StartsAt < estimatedEnd &&
            b.EndsAt > now);

        if (hasConflict)
        {
            logger.LogWarning("Room {RoomId} already has a booking overlapping with manual session {SessionId}.", room.Id, session.Id);
            return Result.Failure(new Error("Room.BookingConflict", "Já existe um agendamento registrado para esta sala nesse horário."));
        }

        session.RoomId = room.Id;
        session.InitiedAt = now;

        await sessionRepository.UpdateAsync(session, cancellationToken);

        await mqttProducer.PublishAsync(room.MicrocontrollerId.ToLower(), session.ToStartSessionEvent(), cancellationToken);

        logger.LogInformation("Manual attendance started for session {SessionId} in room {RoomId} via MQTT.", session.Id, room.Id);

        return Result.Success();
    }
}