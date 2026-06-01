using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public class StartSessionCommandHandler(
    ISessionRepository sessionRepository,
    ISubjectRepository subjectRepository,
    IUserRepository userRepository,
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

        var professor = await userRepository.GetAsync(x => x.TagId == command.Request.ProfessorTagId && x.Role == UserRole.Professor, cancellationToken);
        if (professor is null)
        {
            logger.LogError("Professor with tagId {ProfessorTagId} - not found", command.Request.ProfessorTagId);
            return Result<StartSessionResponse>.Failure(UserErrors.ProfessorNotFound);
        }

        if (!subject.ProfessorId.Equals(professor.Id))
        {
            logger.LogError("Professor {ProfessorId} does not teach the provided Subject {SubjectId}", professor.Id, subject.Id);
            return Result<StartSessionResponse>.Failure(SubjectErrors.ProfessorMismatch);
        }

        var newSession = new Session(
            DateTime.UtcNow,
            command.Request.NumberOfClasses,
            subject.Id,
            professor.Id,
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
