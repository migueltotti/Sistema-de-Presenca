using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.CloseRoom;

public sealed class CloseRoomByTagIdCommandHandler(
    IRoomRepository roomRepository,
    IUserRepository userRepository,
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    ISessionRepository sessionRepository,
    IMqttProducer mqttProducer,
    ILogger<CloseRoomByTagIdCommandHandler> logger) : ICommandHandler<CloseRoomByTagIdCommand, Result>
{
    public async Task<Result> HandleAsync(CloseRoomByTagIdCommand command, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetOneAsync(room => room.MicrocontrollerId == command.MicrocontrollerId, cancellationToken: cancellationToken);
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

        var user = await userRepository.GetOneAsync(user => user.TagId == command.TagId, cancellationToken: cancellationToken);
        if (user is null)
        {
            logger.LogError("User with tag id {TagId} not found.", command.TagId);
            return Result.Failure(UserErrors.NotFound);
        }

        if (user.Role != UserRole.Admin) // 5.1
        {
            if (command.SessionRequest.SessionId.HasValue && user.Role == UserRole.Professor) // 5.2
            {
                var session = await sessionRepository.GetOneAsync(x => 
                    x.Id == command.SessionRequest.SessionId.Value &&
                    x.FinalizedAt == null,
                    includes: ["Professor"],
                    cancellationToken: cancellationToken);

                if (session is null)
                {
                    logger.LogError("Session with id {SessionId} not found.", command.SessionRequest.SessionId.Value);
                    return Result.Failure(SessionErrors.NotFound);
                }

                if (session.RoomId != room.Id)
                {
                    logger.LogError("Session with id {SessionId} does not belong to room {RoomName}.", command.SessionRequest.SessionId.Value, command.MicrocontrollerId);
                    return Result.Failure(SessionErrors.NotFound); // TOOD: Create a specific error for this case
                }

                if (session.Professor.TagId != command.TagId)
                {
                    logger.LogError("User with tag id {TagId} is not the professor for session {SessionId}.", command.TagId, command.SessionRequest.SessionId.Value);
                    return Result.Failure(SessionErrors.ProfessorMismatch);
                }

                session.FinalizedAt = DateTime.UtcNow;

                await sessionRepository.UpdateAsync(session, cancellationToken);

                await mqttProducer.PublishAsync(room.MicrocontrollerId.ToLower(), session.ToEndSessionEvent(), cancellationToken);

                logger.LogInformation("Session with id {SessionId} finalized successfully.", command.SessionRequest.SessionId.Value);
            }
            else // 5.3
            {
                var permission = await roomAccessPermissionRepository.GetOneAsync(x =>
                    x.RoomId == room.Id &&
                    x.User.TagId == command.TagId &&
                    x.Active,
                    cancellationToken: cancellationToken);

                if (permission is null || !permission.Active)
                {
                    logger.LogError("User with tag id {TagId} has no active permission for room {RoomName}.", command.TagId, command.MicrocontrollerId);
                    return Result.Failure(RoomAccessPermissionErrors.PermissionInactive);
                }
            }
        }

        // TODO: pensar se devemos salvar o estado da porta estar trancada ou aberta.
        // pensei até do esp32 enviar periodicamente (a cada 15 s, 30 s, 1 min, 5 min sla) o estado da sala (se a porta está trancada e se tem aula ativa)

        logger.LogInformation("Room {RoomId} is authorized to close for user with tagId {TagId} and role {UserRole}.", room.Id, command.TagId, user.Role);

        return Result.Success();
    }
}
