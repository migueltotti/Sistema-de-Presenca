using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed class DeleteSubjectCommandHandler(
    IUserContextProvider userContextProvider,
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

        await subjectRepository.DeleteAsync(subject, userContextProvider.User.Id, cancellationToken);

        logger.LogInformation("Subject with id {Id} deleted successfully.", command.Id);

        return Result.Success();
    }
}
