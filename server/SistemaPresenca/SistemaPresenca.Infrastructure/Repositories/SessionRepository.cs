using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using System.Linq.Expressions;

namespace SistemaPresenca.Infrastructure.Repositories;

public class SessionRepository(SistemaPresencaDbContext context) : BaseRepository<Session>(context), ISessionRepository
{
    public async Task<Session?> GetWithAttendancesAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await context.Sessions
            .AsNoTracking()
            .Include(s => s.Attendances)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
    }
}
