using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface ISessionRepository : IBaseRepository<Session>
{
    Task<Session?> GetWithAttendancesAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
