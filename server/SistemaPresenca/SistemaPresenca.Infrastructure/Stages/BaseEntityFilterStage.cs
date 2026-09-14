using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Stages;

public static class BaseEntityFilterStage
{
    public static IQueryable<T> ApplyOnlyActiveEntitiesFilter<T>(this IQueryable<T> query) where T : BaseEntity
    {
        return query.Where(entity => entity.DeletedAt == null);
    }
}
