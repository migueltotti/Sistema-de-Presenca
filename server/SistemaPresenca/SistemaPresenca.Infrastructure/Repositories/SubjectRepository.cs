using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;

namespace SistemaPresenca.Infrastructure.Repositories;

public class SubjectRepository(SistemaPresencaDbContext context) : BaseRepository<Subject>(context), ISubjectRepository
{
    public async Task<Subject?> GetWithStudents(Guid subjectId, CancellationToken cancellationToken = default)
    {
        return await context.Subjects
            .AsNoTracking()
            .Include(x => x.Students)
            .FirstOrDefaultAsync(x => x.Id == subjectId, cancellationToken);
    }
}
