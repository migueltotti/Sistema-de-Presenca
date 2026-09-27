using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using SistemaPresenca.Infrastructure.Stages;

namespace SistemaPresenca.Infrastructure.Repositories;

public class RoomAccessPermissionRepository(SistemaPresencaDbContext context)
    : BaseRepository<RoomAccessPermission>(context), IRoomAccessPermissionRepository
{
    public async Task<RoomAccessPermission?> GetActivePermissionAsync(
        Guid userId,
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        return await context.RoomAccessPermissions
            .AsNoTracking()
            .ApplyOnlyActiveEntitiesFilter()
            .FirstOrDefaultAsync(
                permission => permission.UserId == userId && permission.RoomId == roomId && permission.Active,
                cancellationToken);
    }
}