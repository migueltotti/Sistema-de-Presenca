using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface IRoomAccessPermissionRepository : IBaseRepository<RoomAccessPermission>
{
    Task<RoomAccessPermission?> GetActivePermissionAsync(
        Guid userId,
        Guid roomId,
        CancellationToken cancellationToken = default);
}