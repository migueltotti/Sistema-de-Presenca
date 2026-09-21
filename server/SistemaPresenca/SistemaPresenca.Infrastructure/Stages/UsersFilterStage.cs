using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Infrastructure.Stages;

public static class UsersFilterStage
{
    public static IQueryable<User> FilterUsers(this IQueryable<User> pipeline, UserFilters filter)
    {
        var filters = new List<Func<IQueryable<User>, UserFilters, IQueryable<User>>>()
        {
            MatchByIds,
            MatchByNames,
            MatchByEmails,
            MatchByRegistrationIds,
            MatchByCpfs,
            MatchByTagIds,
            MatchByRoles
        };

        filters.ForEach(method =>
            pipeline = method(pipeline, filter));

        return pipeline;
    }

    private static IQueryable<User> MatchByIds(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.Ids != null && filter.Ids.Any())
        {
            pipeline = pipeline.Where(x => filter.Ids.Contains(x.Id));
        }

        return pipeline;
    }

    private static IQueryable<User> MatchByNames(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.Names != null && filter.Names.Any())
        {
            foreach (var filterName in filter.Names)
            {
                pipeline = pipeline.Where(x => x.Name.Contains(filterName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        return pipeline;
    }

    private static IQueryable<User> MatchByEmails(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.Emails != null && filter.Emails.Any())
        {
            pipeline = pipeline.Where(x => filter.Emails.Contains(x.Email));
        }

        return pipeline;
    }

    private static IQueryable<User> MatchByRegistrationIds(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.RegistrationIds != null && filter.RegistrationIds.Any())
        {
            pipeline = pipeline.Where(x => filter.RegistrationIds.Contains(x.RegistrationId));
        }

        return pipeline;
    }

    private static IQueryable<User> MatchByCpfs(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.Cpfs != null && filter.Cpfs.Any())
        {
            pipeline = pipeline.Where(x => filter.Cpfs.Contains(x.Cpf));
        }

        return pipeline;
    }

    private static IQueryable<User> MatchByTagIds(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.TagIds != null && filter.TagIds.Any())
        {
            pipeline = pipeline.Where(x => filter.TagIds.Contains(x.TagId));
        }

        return pipeline;
    }

    private static IQueryable<User> MatchByRoles(IQueryable<User> pipeline, UserFilters filter)
    {
        if (filter.UserRoles != null && filter.UserRoles.Any())
        {
            pipeline = pipeline.Where(x => filter.UserRoles.Contains(x.Role));
        }

        return pipeline;
    }
}
