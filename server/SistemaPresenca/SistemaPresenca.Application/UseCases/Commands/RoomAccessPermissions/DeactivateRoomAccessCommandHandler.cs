using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed class DeactivateRoomAccessCommandHandler(
    IRoomAccessPermissionRepository roomAccessPermissionRepository,
    ILogger<DeactivateRoomAccessCommandHandler> logger) : ICommandHandler<DeactivateRoomAccessCommand, Result>
{
    public async Task<Result> HandleAsync(DeactivateRoomAccessCommand command, CancellationToken cancellationToken = default)
    {
        var permission = await roomAccessPermissionRepository.GetOneAsync(
            permission => permission.Id == command.Id,
            cancellationToken);

        if (permission is null)
        {
            logger.LogError("Room access permission with id {Id} not found.", command.Id);
            return Result.Failure(RoomAccessPermissionErrors.NotFound);
        }

        permission.Active = false;
        await roomAccessPermissionRepository.UpdateAsync(permission, cancellationToken);

        logger.LogInformation("Room access permission with id {Id} deactivated.", command.Id);

        return Result.Success();
    }
}