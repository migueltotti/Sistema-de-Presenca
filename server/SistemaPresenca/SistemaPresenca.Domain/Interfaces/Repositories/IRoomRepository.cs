using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface IRoomRepository : IBaseRepository<Room>
{
    Task<IEnumerable<Room>> GetRoomsAsync(RoomFilters filters, CancellationToken cancellationToken = default);
}