using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;

namespace SistemaPresenca.Infrastructure.Repositories;

public class SessionRepository(SistemaPresencaDbContext context) : BaseRepository<Session>(context), ISessionRepository
{
}
