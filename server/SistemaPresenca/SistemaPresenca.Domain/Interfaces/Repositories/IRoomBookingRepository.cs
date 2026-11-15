using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface IRoomBookingRepository : IBaseRepository<RoomBooking>
{
    Task<IEnumerable<RoomBooking>> GetUpcomingByRoomIdAsync(
        Guid roomId,
        DateTime referenceTime,
        CancellationToken cancellationToken = default);
}