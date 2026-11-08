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

    public static UpdateUserRequest ToUpdateRequest(this User user)
    {
        return new UpdateUserRequest(
            user.Name,
            user.Email,
            user.TagId);
    }

    public static void UpdatedEntity(this UpdateUserRequest request, User user)
    {
        user.Name = request.Name;
        user.Email = request.Email;
        user.TagId = request.TagId;
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

    public static LoginResponse ToLoginResponse(this string accessToken)
    {
        return new LoginResponse(accessToken);
    }
}
