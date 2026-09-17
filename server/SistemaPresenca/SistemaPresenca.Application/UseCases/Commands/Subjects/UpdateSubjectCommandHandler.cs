using FluentValidation;
using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public class UpdateSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IValidator<UpdateSubjectRequest> validator,
    ILogger<UpdateSubjectCommandHandler> logger) : ICommandHandler<UpdateSubjectCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateSubjectCommand command, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetOneAsync(x => x.Id == command.Id, cancellationToken);

        if (subject is null)
        {
            logger.LogError("Subject with id {Id} not found.", command.Id);
            return Result.Failure(SubjectErrors.NotFound);
        }

        var subjectUpdateRequest = subject.ToUpdateRequest();

        command.
            PatchRequest
            .ApplyTo(subjectUpdateRequest);

        var validationResult = validator.Validate(subjectUpdateRequest);
        if (!validationResult.IsValid)
        {
            logger.LogError("Invalid subject update request for subject with id {Id}.", command.Id);
            return Result.Failure(SubjectErrors.InvalidUpdateRequest(string.Join(",", validationResult.Errors)));
        }

        subjectUpdateRequest.UpdatedEntity(subject);

        var subjectWithSameCode = await subjectRepository.GetOneAsync(x => x.Code == subjectUpdateRequest.Code && x.Id != command.Id, cancellationToken);

        if (subjectWithSameCode is not null)
        {
            logger.LogError("Subject with code {Code} already exists.", subjectUpdateRequest.Code);
            return Result.Failure(SubjectErrors.CodeAlreadyExists);
        }

        await subjectRepository.UpdateAsync(subject, cancellationToken);

        logger.LogInformation("Subject with id {Id} updated successfully.", command.Id);

        return Result.Success();
    }
}
