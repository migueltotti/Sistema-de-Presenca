using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;

namespace SistemaPresenca.Infrastructure.Repositories;

public class MajorRepoitory(SistemaPresencaDbContext context) : BaseRepository<Major>(context), IMajorRepoitory
{
}
