using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed class GrantRoomAccessCommandHandler(
    IUserContextProvider userContextProvider,
    IUserRepository userRepository,
    IRoomRepository roomRepository,
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    ILogger<GrantRoomAccessCommandHandler> logger) : ICommandHandler<GrantRoomAccessCommand, Result>
{
    public async Task<Result> HandleAsync(GrantRoomAccessCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetOneAsync(user => user.Id == command.UserId, cancellationToken: cancellationToken);

        if (user is null)
        {
            logger.LogError("User with id {UserId} not found.", command.UserId);
            return Result.Failure(RoomAccessPermissionErrors.UserNotFound);
        }

        var room = await roomRepository.GetOneAsync(room => room.Id == command.RoomId, cancellationToken: cancellationToken);

        if (room is null)
        {
            logger.LogError("Room with id {RoomId} not found.", command.RoomId);
            return Result.Failure(RoomAccessPermissionErrors.RoomNotFound);
        }

        var existingPermission = await roomAccessPermissionRepository.GetOneAsync(permission =>
            permission.UserId == command.UserId && 
            permission.RoomId == command.RoomId,
            cancellationToken: cancellationToken);

        if (existingPermission is not null)
        {
            logger.LogError("Access permission already exists for user {UserId} and room {RoomId}.", command.UserId, command.RoomId);
            return Result.Failure(RoomAccessPermissionErrors.AlreadyExists);
        }

        var permission = command.ToEntity(userContextProvider.User.Id);

        await roomAccessPermissionRepository.AddAsync(permission, cancellationToken);

        logger.LogInformation("Access permission {Id} granted to user {UserId} for room {RoomId}.", permission.Id, command.UserId, command.RoomId);

        return Result.Success();
    }
}