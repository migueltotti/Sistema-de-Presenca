using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string RegistrationId { get; set; }
    public string? Cpf { get; set; }
    public string? TagId { get; set; }
    public UserRole Role { get; set; }

    private User() : base()
    {
    }

    public User(string name, string email, string password, string registrationId, string? cpf, string? tagId, UserRole role, Guid? createdByAdminId) : base(createdByAdminId)
    {
        Name = name;
        Email = email;
        Password = password;
        RegistrationId = registrationId;
        Cpf = cpf;
        TagId = tagId;
        Role = role;
    }
}
