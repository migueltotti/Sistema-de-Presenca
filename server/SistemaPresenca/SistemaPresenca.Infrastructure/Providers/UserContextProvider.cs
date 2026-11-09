using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Infrastructure.Providers;

public sealed class UserContextProvider : IUserContextProvider
{
    public Guid UserId => _userId;
    public string UserEmail => _userEmail;
    public UserRole? UserRole => _userRole;

    private Guid _userId = Guid.Empty;
    private string _userEmail = string.Empty;
    private UserRole? _userRole = null;

    public void SetUserId(Guid? id)
    {
        if (id != null && id != Guid.Empty)
        {
            _userId = id.Value;
        }
    }

    public void SetUserEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            _userEmail = email;
        }
    }

    public void SetUserRole(UserRole? role)
    {
        _userRole = role;
    }
}
