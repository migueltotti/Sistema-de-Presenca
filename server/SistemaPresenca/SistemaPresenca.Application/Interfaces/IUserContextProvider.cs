using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Interfaces;

public interface IUserContextProvider
{
    public Guid UserId { get; }
    public string UserEmail { get; }
    public UserRole? UserRole { get; }
    void SetUserId(Guid? id);
    void SetUserEmail(string? email);
    void SetUserRole(UserRole? role);
}
