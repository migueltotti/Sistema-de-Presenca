using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;

namespace SistemaPresenca.Infrastructure.Repositories;

public class RoomBookingRepository(SistemaPresencaDbContext context)
    : BaseRepository<RoomBooking>(context), IRoomBookingRepository
{
    public async Task<IEnumerable<RoomBooking>> GetUpcomingByRoomIdAsync(
        Guid roomId,
        DateTime referenceTime,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<RoomBooking>()
            .Include(b => b.Subject)
                .ThenInclude(s => s!.Professors)
            .Include(b => b.Session)
            .Where(b =>
                b.RoomId == roomId &&
                b.Status == RoomBookingStatus.Scheduled &&
                b.EndsAt > referenceTime)
            .OrderBy(b => b.StartsAt)
            .ToListAsync(cancellationToken);
    }
}