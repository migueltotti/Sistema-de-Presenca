using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Infrastructure.Stages;

public static class SubjectFilterStage
{
    public static IQueryable<Subject> FilterSubjects(this IQueryable<Subject> pipeline, SubjectFilters filter)
    {
        var filters = new List<Func<IQueryable<Subject>, SubjectFilters, IQueryable<Subject>>>()
        {
            MatchByIds,
            MatchByNames,
            MatchByCodes,
            MatchByMajorIds,
            MatchByProfessorIds
        };

        filters.ForEach(method =>
            pipeline = method(pipeline, filter));

        return pipeline;
    }

    private static IQueryable<Subject> MatchByIds(IQueryable<Subject> pipeline, SubjectFilters filter)
    {
        if (filter.Ids != null && filter.Ids.Any())
        {
            pipeline = pipeline.Where(x => filter.Ids.Contains(x.Id));
        }

        return pipeline;
    }

    private static IQueryable<Subject> MatchByNames(IQueryable<Subject> pipeline, SubjectFilters filter)
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

    private static IQueryable<Subject> MatchByCodes(IQueryable<Subject> pipeline, SubjectFilters filter)
    {
        if (filter.Codes != null && filter.Codes.Any())
        {
            pipeline = pipeline.Where(x => filter.Codes.Contains(x.Code));
        }

        return pipeline;
    }

    private static IQueryable<Subject> MatchByMajorIds(IQueryable<Subject> pipeline, SubjectFilters filter)
    {
        if (filter.MajorIds != null && filter.MajorIds.Any())
        {
            pipeline = pipeline.Where(x => filter.MajorIds.Contains(x.MajorId));
        }

        return pipeline;
    }

    private static IQueryable<Subject> MatchByProfessorIds(IQueryable<Subject> pipeline, SubjectFilters filter)
    {
        if (filter.ProfessorIds != null && filter.ProfessorIds.Any())
        {
            foreach (var professorId in filter.ProfessorIds)
            {
                pipeline = pipeline.Where(x => x.ProfessorIds.Contains(professorId));
            }
        }

        return pipeline;
    }
}