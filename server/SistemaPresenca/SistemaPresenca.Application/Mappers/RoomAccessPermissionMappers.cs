using SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class RoomAccessPermissionMappers
{
    public static RoomAccessPermission ToEntity(this GrantRoomAccessCommand command, Guid createdByAdminId)
    {
        return new RoomAccessPermission(
            command.UserId,
            command.RoomId,
            createdByAdminId);
    }
}
