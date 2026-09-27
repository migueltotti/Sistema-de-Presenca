using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed class RevokeRoomAccessCommandHandler(
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    ILogger<RevokeRoomAccessCommandHandler> logger) : ICommandHandler<RevokeRoomAccessCommand, Result>
{
    public async Task<Result> HandleAsync(RevokeRoomAccessCommand command, CancellationToken cancellationToken = default)
    {
        var permission = await roomAccessPermissionRepository.GetOneAsync(
            permission => permission.Id == command.Id,
            cancellationToken);

        if (permission is null)
        {
            logger.LogError("Room access permission with id {Id} not found.", command.Id);
            return Result.Failure(RoomAccessPermissionErrors.NotFound);
        }

        await roomAccessPermissionRepository.DeleteAsync(
            permission,
            Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c"),
            cancellationToken);

        logger.LogInformation("Room access permission with id {Id} revoked.", command.Id);

        return Result.Success();
    }
}