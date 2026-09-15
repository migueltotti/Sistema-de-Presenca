using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed class DeleteSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    ILogger<DeleteSubjectCommandHandler> logger) : ICommandHandler<DeleteSubjectCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteSubjectCommand command, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetOneAsync(x => x.Id == command.Id, cancellationToken);

        if (subject is null)
        {
            logger.LogError("Subject with id {Id} not found.", command.Id);
            return Result.Failure(SubjectErrors.NotFound);
        }

        await subjectRepository.DeleteAsync(subject, Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c"), cancellationToken);

        logger.LogInformation("Subject with id {Id} deleted successfully.", command.Id);

        return Result.Success();
    }
}
