namespace SistemaPresenca.Domain.Filters;

public class MajorFilters
{
    public IEnumerable<Guid>? Ids { get; private set; }
    public IEnumerable<string>? Names { get; private set; }
    public IEnumerable<string>? Codes { get; private set; }

    public class Builder {
        private readonly MajorFilters _filters = new();

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

        public Builder WithCode(IEnumerable<string>? code)
        {
            _filters.Codes = code;
            return this;
        }

        public MajorFilters Build()
        {
            return _filters;
        }
    }
}
