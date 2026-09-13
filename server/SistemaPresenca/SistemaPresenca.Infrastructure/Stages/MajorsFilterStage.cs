using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Infrastructure.Stages;

public static class MajorsFilterStage
{
    public static IQueryable<Major> FilterMajors(this IQueryable<Major> pipeline, MajorFilters filter)
    {
        var filters = new List<Func<IQueryable<Major>, MajorFilters, IQueryable<Major>>>()
        {
            MatchByIds,
            MatchByNames,
            MatchByCodes,
        };

        filters.ForEach(method =>
            pipeline = method(pipeline, filter));

        return pipeline;
    }

    private static IQueryable<Major> MatchByIds(IQueryable<Major> pipeline, MajorFilters filter)
    {
        if (filter.Ids != null && filter.Ids.Any())
        {
            pipeline = pipeline.Where(x => filter.Ids.Contains(x.Id));
        }

        return pipeline;
    }

    private static IQueryable<Major> MatchByNames(IQueryable<Major> pipeline,MajorFilters filter)
    {
        if (filter.Names != null && filter.Names.Any())
        {
            foreach (var filterName in filter.Names)
            {
                pipeline = pipeline.Where(x => x.Name.Contains(filterName));
            }
        }

        return pipeline;
    }

    private static IQueryable<Major> MatchByCodes(IQueryable<Major> pipeline, MajorFilters filter)
    {
        if (filter.Codes != null && filter.Codes.Any())
        {
            pipeline = pipeline.Where(x => filter.Codes.Contains(x.Code));
        }

        return pipeline;
    }
}
