namespace SistemaPresenca.Application.Requests.Users;

public record UpdateUserRequest(
    string Name,
    string Email,
    string? TagId
);
