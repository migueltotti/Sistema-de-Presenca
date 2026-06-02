using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;
using System.Linq.Expressions;

namespace SistemaPresenca.Infrastructure.Repositories;

public class BaseRepository<T>(SistemaPresencaDbContext context)
    : IBaseRepository<T> where T : class
{
    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(expression, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await context.Set<T>().AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        context.Set<T>().Update(entity);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        context.Set<T>().Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
