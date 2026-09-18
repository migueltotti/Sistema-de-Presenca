using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class UserMappers
{
    public static User ToEntity(this CreateUserRequest request)
    {
        return new User(
            request.Name,
            request.Email,
            request.RegistrationId,
            request.Cpf,
            request.Role,
            Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c"));
    }
}
