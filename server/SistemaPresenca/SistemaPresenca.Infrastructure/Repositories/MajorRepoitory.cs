using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using SistemaPresenca.Infrastructure.Stages;

namespace SistemaPresenca.Infrastructure.Repositories;

public class MajorRepoitory(SistemaPresencaDbContext context) : BaseRepository<Major>(context), IMajorRepoitory
{
    private readonly SistemaPresencaDbContext _context = context;

    public async Task<IEnumerable<Major>> GetMajorsAsync(MajorFilters filters, CancellationToken cancellationToken)
    {
        return await _context.Majors
            .AsQueryable()
            .AsNoTracking()
            .FilterMajors(filters)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
