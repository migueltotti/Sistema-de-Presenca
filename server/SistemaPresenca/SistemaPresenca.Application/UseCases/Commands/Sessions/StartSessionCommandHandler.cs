using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public class StartSessionCommandHandler(
    ISessionRepository sessionRepository,
    ISubjectRepository subjectRepository,
    ILogger<StartSessionCommandHandler> logger) : ICommandHandler<StartSessionCommand, Result<StartSessionResponse>>
{
    public async Task<Result<StartSessionResponse>> HandleAsync(StartSessionCommand command, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetWithStudents(command.Request.SubjectId, cancellationToken);
        if (subject is null)
        {
            logger.LogError("Subject with id {SubjectId} - not found", command.Request.SubjectId);
            return Result<StartSessionResponse>.Failure(SubjectErrors.NotFound);
        }

        if (!subject.ProfessorId.Equals(command.Request.ProfessorId))
        {
            logger.LogError("Professor {ProfessorId} does not teach the provided Subject {SubjectId}", command.Request.ProfessorId, command.Request.SubjectId);
            return Result<StartSessionResponse>.Failure(SubjectErrors.ProfessorMismatch);
        }

        var newSession = new Session(
            DateTime.UtcNow,
            command.Request.NumberOfClasses,
            command.Request.SubjectId,
            command.Request.ProfessorId,
            null
        );

        foreach (var student in subject.Students)
        {
            newSession.Attendances.Add(
                new Attendance(
                    newSession.Id,
                    student.Id)
            );
        }

        await sessionRepository.AddAsync(newSession, cancellationToken);

        return Result<StartSessionResponse>.Success(new StartSessionResponse(newSession.Id));
    }
}
