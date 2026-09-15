using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Majors;

public sealed class DeleteMajorCommandHandler(
    IMajorRepository majorRepoitory,
    ILogger<DeleteMajorCommandHandler> logger) : ICommandHandler<DeleteMajorCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteMajorCommand command, CancellationToken cancellationToken = default)
    {
        var major = await majorRepoitory.GetOneAsync(x => x.Id == command.Id, cancellationToken);

        if (major is null)
        {
            logger.LogError("Major with id {Id} not found.", command.Id);
            return Result.Failure(MajorErrors.NotFound);
        }

        await majorRepoitory.DeleteAsync(major, Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c"), cancellationToken);

        logger.LogInformation("Major with id {Id} deleted successfully.", command.Id);

        return Result.Success();
    }
}
