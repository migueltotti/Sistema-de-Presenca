using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Application.Responses.Users;
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

    public static GetUserResponse ToResponse(this User user)
    {
        return new GetUserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.RegistrationId,
            user.Cpf,
            user.TagId,
            user.Role);
    }
}
