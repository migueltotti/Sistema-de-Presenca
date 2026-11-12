using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public class StartSessionCommandHandler(
    IUserContextProvider userContextProvider,
    ISessionRepository sessionRepository,
    ISubjectRepository subjectRepository,
    IRoomRepository roomRepository,
    IUserRepository userRepository,
    ILogger<StartSessionCommandHandler> logger) : ICommandHandler<StartSessionCommand, Result<StartSessionResponse>>
{
    public async Task<Result<StartSessionResponse>> HandleAsync(StartSessionCommand command, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetOneAsync(x => x.Id == command.Request.SubjectId, cancellationToken: cancellationToken);
        if (subject is null)
        {
            logger.LogError("Subject with id {SubjectId} - not found", command.Request.SubjectId);
            return Result<StartSessionResponse>.Failure(SubjectErrors.NotFound);
        }

        var room = await roomRepository.GetOneAsync(x => x.Id == command.Request.RoomId, cancellationToken: cancellationToken);
        if (room is null)
        {
            logger.LogError("Room with id {RoomId} - not found", command.Request.RoomId);
            return Result<StartSessionResponse>.Failure(RoomErrors.NotFound);
        }

        var professor = await userRepository.GetOneAsync(x => x.TagId == command.Request.ProfessorTagId && x.Role == UserRole.Professor, cancellationToken: cancellationToken);
        if (professor is null)
        {
            logger.LogError("Professor with tagId {ProfessorTagId} - not found", command.Request.ProfessorTagId);
            return Result<StartSessionResponse>.Failure(UserErrors.ProfessorNotFound);
        }

        if (!subject.ProfessorIds.Contains(professor.Id))
        {
            logger.LogError("Professor {ProfessorId} does not teach the provided Subject {SubjectId}", professor.Id, subject.Id);
            return Result<StartSessionResponse>.Failure(SubjectErrors.ProfessorMismatch);
        }

        var newSession = new Session(
            DateTime.UtcNow,
            command.Request.NumberOfClasses,
            subject.Id,
            room.Id,
            //professor.Id,
            userContextProvider.User.Id
        );

        foreach (var student in subject.Students ?? [])
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
