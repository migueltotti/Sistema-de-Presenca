using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed class RevokeRoomAccessCommandHandler(
    IUserContextProvider userContextProvider,
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    ILogger<RevokeRoomAccessCommandHandler> logger) : ICommandHandler<RevokeRoomAccessCommand, Result>
{
    public async Task<Result> HandleAsync(RevokeRoomAccessCommand command, CancellationToken cancellationToken = default)
    {
        var permission = await roomAccessPermissionRepository.GetOneAsync(permission => 
            permission.Id == command.PermissionId && 
            permission.RoomId == command.RoomId,
            cancellationToken: cancellationToken);

        if (permission is null)
        {
            logger.LogError("Room access permission with id {Id} not found.", command.PermissionId);
            return Result.Failure(RoomAccessPermissionErrors.NotFound);
        }

        await roomAccessPermissionRepository.DeleteAsync(
            permission,
            userContextProvider.User.Id,
            cancellationToken);

        logger.LogInformation("Room access permission with id {Id} revoked.", command.PermissionId);

        return Result.Success();
    }
}