using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Models;

public class UserContextModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole? Role { get; set; }
}
