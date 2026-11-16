using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Responses.Users;

public record GetUserResponse(
    Guid Id,
    string Name,
    string Email,
    string RegistrationId,
    string Cpf,
    string? TagId,
    UserRole Role,
    IEnumerable<GetUserAccessResponse> AccessPermissions
);

public sealed record GetUserAccessResponse(
    Guid RoomId,
    DateTime GrantedAt,
    bool Active
);