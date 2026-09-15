using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed class CreateSubjectCommandHandler(
    IMajorRepository majorRepository,
    ISubjectRepository subjectRepository,
    ILogger<CreateSubjectCommandHandler> logger) : ICommandHandler<CreateSubjectCommand, Result>
{
    public async Task<Result> HandleAsync(CreateSubjectCommand command, CancellationToken cancellationToken = default)
    {
        var major = await majorRepository.GetOneAsync(x => x.Id == command.Request.MajorId, cancellationToken);

        if (major is null)
        {
            logger.LogError("Major with id {Id} not found.", command.Request.MajorId);
            return Result.Failure(MajorErrors.NotFound);
        }

        var subjectWithSameCode = await subjectRepository.GetOneAsync(x => x.Code == command.Request.Code, cancellationToken);

        if (subjectWithSameCode is not null)
        {
            logger.LogError("Subject with code {Code} already exists.", command.Request.Code);
            return Result.Failure(SubjectErrors.CodeAlreadyExists);
        }

        var newSubject = command.Request.ToEntity();

        await subjectRepository.AddAsync(newSubject, cancellationToken);

        logger.LogInformation("Subject with id {Id} created successfully.", newSubject.Id);

        return Result.Success();
    }
}