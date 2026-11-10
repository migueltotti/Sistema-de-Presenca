using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public class DeleteUserCommandHandler(
    IUserContextProvider userContextProvider,
    IUserRepository userRepository,
    ILogger<DeleteUserCommandHandler> logger) : ICommandHandler<DeleteUserCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetOneAsync(x => x.Id == command.Id, cancellationToken);

        if (user is null)
        {
            logger.LogError("User with id {Id} not found.", command.Id);
            return Result.Failure(UserErrors.NotFound);
        }

        await userRepository.DeleteAsync(user, userContextProvider.User.Id, cancellationToken);

        logger.LogInformation("User with id {Id} deleted successfully.", command.Id);

        return Result.Success();
    }
}
