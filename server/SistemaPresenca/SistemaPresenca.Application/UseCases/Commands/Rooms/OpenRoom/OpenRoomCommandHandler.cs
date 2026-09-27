using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.OpenRoom;

public sealed class OpenRoomCommandHandler(
    IRoomRepository roomRepository,
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    ILogger<OpenRoomCommandHandler> logger) : ICommandHandler<OpenRoomCommand, Result>
{
    public async Task<Result> HandleAsync(OpenRoomCommand command, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetOneAsync(room => room.Id == command.RoomId, cancellationToken);

        if (room is null)
        {
            logger.LogError("Room with id {RoomId} not found.", command.RoomId);
            return Result.Failure(RoomAccessPermissionErrors.RoomNotFound);
        }

        if (room.Status != RoomStatus.Active)
        {
            logger.LogError("Room with id {RoomId} is inactive.", command.RoomId);
            return Result.Failure(RoomAccessPermissionErrors.RoomInactive);
        }

        var permission = await roomAccessPermissionRepository.GetActivePermissionAsync(
            command.UserId,
            command.RoomId,
            cancellationToken);

        if (permission is null || !permission.Active)
        {
            logger.LogError("User {UserId} has no active permission for room {RoomId}.", command.UserId, command.RoomId);
            return Result.Failure(RoomAccessPermissionErrors.PermissionInactive);
        }

        // TODO: disparar comando de abertura via mensageria para o microcontrolador desta sala (implementar em conjunto — Parte 3)
        logger.LogInformation("Room {RoomId} is authorized to open for user {UserId}.", command.RoomId, command.UserId);

        return Result.Success();
    }
}