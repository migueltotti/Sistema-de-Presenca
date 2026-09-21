using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Domain.Filters;

public class UserFilters
{
    public IEnumerable<Guid>? Ids { get; private set; }
    public IEnumerable<string>? Names { get; private set; }
    public IEnumerable<string>? Emails { get; private set; }
    public IEnumerable<string>? RegistrationIds { get; private set; }
    public IEnumerable<string>? Cpfs { get; private set; }
    public IEnumerable<string>? TagIds { get; private set; }
    public IEnumerable<UserRole>? UserRoles { get; private set; }

    public class Builder
    {
        private readonly UserFilters _filters = new();

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

        public Builder WithEmails(IEnumerable<string>? emails)
        {
            _filters.Emails = emails;
            return this;
        }

        public Builder WithRegistrationIds(IEnumerable<string>? registrationIds)
        {
            _filters.RegistrationIds = registrationIds;
            return this;
        }

        public Builder WithCpfs(IEnumerable<string>? cpfs)
        {
            _filters.Cpfs = cpfs;
            return this;
        }

        public Builder WithTagIds(IEnumerable<string>? tagIds)
        {
            _filters.TagIds = tagIds;
            return this;
        }

        public Builder WithRoles(IEnumerable<UserRole>? roles)
        {
            _filters.UserRoles = roles;
            return this;
        }

        public UserFilters Build()
        {
            return _filters;
        }
    }
}
