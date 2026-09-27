using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public class DeleteUserCommandHandler(
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

        await userRepository.DeleteAsync(user, Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c"), cancellationToken);

        logger.LogInformation("User with id {Id} deleted successfully.", command.Id);

        return Result.Success();
    }
}
