using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Requests.Users;

public sealed class GetUsersRequest
{
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Emails { get; set; }
    public IEnumerable<string>? RegistrationIds { get; set; }
    public IEnumerable<string>? Cpfs { get; set; }
    public IEnumerable<string>? TagIds { get; set; }
    public IEnumerable<UserRole>? UserRoles { get; set; }
}