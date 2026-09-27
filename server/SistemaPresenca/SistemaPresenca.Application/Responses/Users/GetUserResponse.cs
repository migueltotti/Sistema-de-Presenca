using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Responses.Users;

public record GetUserResponse(
    Guid Id,
    string Name,
    string Email,
    string RegistrationId,
    string Cpf,
    string? TagId,
    UserRole Role
);