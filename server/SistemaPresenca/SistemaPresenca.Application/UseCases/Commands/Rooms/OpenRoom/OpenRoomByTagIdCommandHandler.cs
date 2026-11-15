using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.OpenRoom;

public sealed class OpenRoomByTagIdCommandHandler(
    IRoomRepository roomRepository,
    IUserRepository userRepository,
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    IRoomBookingRepository roomBookingRepository,
    ISessionRepository sessionRepository,
    IMqttProducer mqttProducer,
    ILogger<OpenRoomByTagIdCommandHandler> logger) : ICommandHandler<OpenRoomByTagIdCommand, Result>
{
    private const int UpcomingBookingThresholdMinutes = 50;

    public async Task<Result> HandleAsync(OpenRoomByTagIdCommand command, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetOneAsync(r => r.MicrocontrollerId == command.MicrocontrollerId, cancellationToken: cancellationToken);
        if (room is null)
        {
            logger.LogError("Room with microcontroller id {MicrocontrollerId} not found.", command.MicrocontrollerId);
            return Result.Failure(RoomAccessPermissionErrors.RoomNotFound);
        }

        if (room.Status != RoomStatus.Active)
        {
            logger.LogError("Room with microcontroller id {MicrocontrollerId} is inactive.", command.MicrocontrollerId);
            return Result.Failure(RoomAccessPermissionErrors.RoomInactive);
        }

        var user = await userRepository.GetOneAsync(u => u.TagId == command.TagId, cancellationToken: cancellationToken);
        if (user is null)
        {
            logger.LogError("User with tag id {TagId} not found.", command.TagId);
            return Result.Failure(UserErrors.NotFound);
        }

        // 5.1 Se o usuário for Admin, retorna sucesso para abrir a porta.
        if (user.Role == UserRole.Admin)
        {
            logger.LogInformation("Room {RoomId} authorized to open for Admin with tagId {TagId}.", room.Id, command.TagId);
            return Result.Success();
        }

        // 5.2 Se tiver aula ativa no ESP32 (SessionId informado) e não for Admin, retorna falha
        if (command.SessionRequest.SessionId.HasValue)
        {
            logger.LogWarning("Room {RoomId} has active session {SessionId}; non-admin user {TagId} cannot unlock via server.",
                room.Id, command.SessionRequest.SessionId.Value, command.TagId);
            return Result.Failure(new Error("Room.ActiveSession", "Aula em andamento na sala."));
        }

        // 5.3 Sem aula ativa: verifica se o usuário tem permissão ativa para acessar a sala
        var permission = await roomAccessPermissionRepository.GetOneAsync(x =>
            x.RoomId == room.Id &&
            x.User.TagId == command.TagId &&
            x.Active,
            cancellationToken: cancellationToken);

        // 5.3.1 Se não tiver permissão, retorna falha
        if (permission is null || !permission.Active)
        {
            logger.LogError("User with tag id {TagId} has no active permission for room {RoomName}.", command.TagId, command.MicrocontrollerId);
            return Result.Failure(RoomAccessPermissionErrors.PermissionInactive);
        }

        // 5.3.2 Busca por agendamentos próximos naquela sala
        var now = DateTime.UtcNow;
        var upcomingBookings = await roomBookingRepository.GetUpcomingByRoomIdAsync(room.Id, now, cancellationToken);
        var nextBooking = upcomingBookings.FirstOrDefault();

        // 5.3.2.1 Se não tiver agendamento próximo, retorna sucesso para abrir a porta
        if (nextBooking is null)
        {
            logger.LogInformation("Room {RoomId} has no upcoming bookings. Authorized to open for tagId {TagId}.", room.Id, command.TagId);
            return Result.Success();
        }

        var minutesUntilBooking = (nextBooking.StartsAt - now).TotalMinutes;
        var isBookingOwner = nextBooking.ProfessorId == user.Id;

        // 5.3.2.2 Verifica se o agendamento é do usuário e se está perto do horário (<= 50 min)
        if (!isBookingOwner)
        {
            if (minutesUntilBooking > UpcomingBookingThresholdMinutes)
            {
                logger.LogInformation("Room {RoomId} next booking is in {Minutes:F0} min (> 50 min). Authorized to open for tagId {TagId}.",
                    room.Id, minutesUntilBooking, command.TagId);
                return Result.Success();
            }

            logger.LogWarning("Room {RoomId} has an upcoming booking in {Minutes:F0} min for another user.", room.Id, minutesUntilBooking);
            return Result.Failure(new Error("Room.UpcomingBookingConflict", "Sala reservada para outro responsável."));
        }

        if (nextBooking.Type == RoomBookingType.Personal)
        {
            logger.LogInformation("Room {RoomId} authorized to open for owner's personal booking {BookingId}.", room.Id, nextBooking.Id);
            return Result.Success();
        }

        if (user.Role != UserRole.Professor)
        {
            return Result.Failure(new Error("Room.InvalidBookingRole", "Apenas professores podem iniciar agendamentos de aula."));
        }

        if (!nextBooking.AutoStartAttendance)
        {
            logger.LogInformation("Room {RoomId} authorized to open (manual attendance configured) for booking {BookingId}.", room.Id, nextBooking.Id);
            return Result.Success();
        }

        var session = nextBooking.Session;
        if (session is null && nextBooking.SubjectId.HasValue)
        {
            var totalLessons = Math.Max(1, (int)Math.Round((nextBooking.EndsAt - nextBooking.StartsAt).TotalMinutes / 50.0));
            session = new Session(now, totalLessons, nextBooking.SubjectId.Value, room.Id, user.Id, nextBooking.Id);
            session.Subject = nextBooking.Subject!;
            await sessionRepository.AddAsync(session, cancellationToken);
        }
        else if (session is not null)
        {
            session.RoomId ??= room.Id;
            session.InitiedAt = now;
            await sessionRepository.UpdateAsync(session, cancellationToken);
        }

        if (session is not null)
        {
            await mqttProducer.PublishAsync(room.MicrocontrollerId.ToLower(), session.ToStartSessionEvent(), cancellationToken);
            logger.LogInformation("Auto-started attendance session {SessionId} for room {RoomId} via MQTT.", session.Id, room.Id);
        }

        return Result.Success();
    }
}