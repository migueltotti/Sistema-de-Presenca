using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using SistemaPresenca.Infrastructure.Stages;

namespace SistemaPresenca.Infrastructure.Repositories;

public class SubjectRepository(SistemaPresencaDbContext context) : BaseRepository<Subject>(context), ISubjectRepository
{
    private readonly SistemaPresencaDbContext _context = context;
    public async Task<IEnumerable<Subject>> GetSubjectsAsync(SubjectFilters filters, CancellationToken cancellationToken = default)
    {
        return await _context.Subjects
            .AsQueryable()
            .AsNoTracking()
            .ApplyOnlyActiveEntitiesFilter()
            .FilterSubjects(filters)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
