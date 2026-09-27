using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using SistemaPresenca.Infrastructure.Stages;

namespace SistemaPresenca.Infrastructure.Repositories;

public class RoomRepository(SistemaPresencaDbContext context) : BaseRepository<Room>(context), IRoomRepository
{
    public async Task<IEnumerable<Room>> GetRoomsAsync(RoomFilters filters, CancellationToken cancellationToken = default)
    {
        return await context.Rooms
            .AsQueryable()
            .AsNoTracking()
            .ApplyOnlyActiveEntitiesFilter()
            .FilterRooms(filters)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}