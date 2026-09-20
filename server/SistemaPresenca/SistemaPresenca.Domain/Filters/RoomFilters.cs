namespace SistemaPresenca.Domain.Filters;

public class RoomFilters
{
    public IEnumerable<Guid>? Ids { get; private set; }
    public IEnumerable<string>? Names { get; private set; }
    public IEnumerable<string>? Locations { get; private set; }

    public class Builder
    {
        private readonly RoomFilters _filters = new();

        public Builder WithIds(IEnumerable<Guid>? ids)
        {
            _filters.Ids = ids;
            return this;
        }

        public Builder WithNames(IEnumerable<string>? names)
        {
            _filters.Names = names;
            return this;
        }

        public Builder WithLocations(IEnumerable<string>? locations)
        {
            _filters.Locations = locations;
            return this;
        }

        public RoomFilters Build()
        {
            return _filters;
        }
    }
}