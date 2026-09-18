using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Requests.Users;

public sealed record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    string RegistrationId,
    string Cpf,
    UserRole Role);