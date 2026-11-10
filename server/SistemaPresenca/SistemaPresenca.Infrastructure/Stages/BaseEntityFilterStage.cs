using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Stages;

public static class BaseEntityFilterStage
{
    public static IQueryable<T> ApplyOnlyActiveEntitiesFilter<T>(this IQueryable<T> query) where T : BaseEntity
    {
        return query.Where(entity => entity.DeletedAt == null);
    }

    public static IQueryable<T> ApplyIncludesFilter<T>(this IQueryable<T> query, IEnumerable<string>? includes) where T : BaseEntity
    {
        foreach (var include in includes ?? [])
        {
            query = query.Include(include);
        }

        return query;
    }
}
