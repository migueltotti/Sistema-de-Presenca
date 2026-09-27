using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using SistemaPresenca.Infrastructure.Stages;

namespace SistemaPresenca.Infrastructure.Repositories;

public class UserRepository(SistemaPresencaDbContext context) : BaseRepository<User>(context), IUserRepository
{
    private readonly SistemaPresencaDbContext _context = context;

    public async Task<IEnumerable<User>> GetUsersAsync(UserFilters filters, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsQueryable()
            .AsNoTracking()
            .ApplyOnlyActiveEntitiesFilter()
            .FilterUsers(filters)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}