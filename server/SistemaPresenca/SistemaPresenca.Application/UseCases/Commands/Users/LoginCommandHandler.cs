using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Users;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Interfaces.Services;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IAuthenticationService authenticationService,
    ILogger<LoginCommandHandler> logger
) : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetOneAsync(x => x.Email == command.Request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("User not found for Email: {Email}", command.Request.Email);

            return Result<LoginResponse>.Failure(AuthenticationErrors.IncorrectCredentials);
        }

        var authenticationResult = authenticationService.Authenticate(user, command.Request.Password);

        if (authenticationResult.IsFailure)
            return Result<LoginResponse>.Failure(authenticationResult.Error);

        return Result<LoginResponse>.Success(authenticationResult.Data.ToLoginResponse());
    }
}
