namespace SistemaPresenca.Domain.Filters;

public class SubjectFilters
{
    public IEnumerable<Guid>? Ids { get; private set; }
    public IEnumerable<string>? Names { get; private set; }
    public IEnumerable<string>? Codes { get; private set; }
    public IEnumerable<Guid>? MajorIds { get; private set; }
    public IEnumerable<Guid>? ProfessorIds { get; private set; }

    public class Builder
    {
        private readonly SubjectFilters _filters = new();

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

        public Builder WithCodes(IEnumerable<string>? code)
        {
            _filters.Codes = code;
            return this;
        }

        public Builder WithMajorIds(IEnumerable<Guid>? majorIds)
        {
            _filters.MajorIds = majorIds;
            return this;
        }

        public Builder WithProfessorIds(IEnumerable<Guid>? professorIds)
        {
            _filters.ProfessorIds = professorIds;
            return this;
        }

        public SubjectFilters Build()
        {
            return _filters;
        }
    }
}
