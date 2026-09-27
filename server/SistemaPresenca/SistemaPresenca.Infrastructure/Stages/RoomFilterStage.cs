using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Infrastructure.Stages;

public static class RoomFilterStage
{
    public static IQueryable<Room> FilterRooms(this IQueryable<Room> pipeline, RoomFilters filter)
    {
        var filters = new List<Func<IQueryable<Room>, RoomFilters, IQueryable<Room>>>()
        {
            MatchByIds,
            MatchByNames,
            MatchByLocations
        };

        filters.ForEach(method =>
            pipeline = method(pipeline, filter));

        return pipeline;
    }

    private static IQueryable<Room> MatchByIds(IQueryable<Room> pipeline, RoomFilters filter)
    {
        if (filter.Ids != null && filter.Ids.Any())
        {
            var ids = filter.Ids.ToArray();
            pipeline = pipeline.Where(x => ids.Contains(x.Id));
        }

        return pipeline;
    }

    private static IQueryable<Room> MatchByNames(IQueryable<Room> pipeline, RoomFilters filter)
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

    private static IQueryable<Room> MatchByLocations(IQueryable<Room> pipeline, RoomFilters filter)
    {
        if (filter.Locations != null && filter.Locations.Any())
        {
            foreach (var filterLocation in filter.Locations)
            {
                pipeline = pipeline.Where(x => x.Location.Contains(filterLocation));
            }
        }

        return pipeline;
    }
}