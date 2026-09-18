using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string RegistrationId { get; set; }
    public string Cpf { get; set; }
    public string? TagId { get; set; }
    public UserRole Role { get; set; }

    private User() : base()
    {
    }

    public User(string name, string email, string registrationId, string cpf, UserRole role, Guid? createdByAdminId) : base(createdByAdminId)
    {
        Name = name;
        Email = email;
        Password = string.Empty;
        RegistrationId = registrationId;
        Cpf = cpf;
        TagId = null;
        Role = role;
    }
}
