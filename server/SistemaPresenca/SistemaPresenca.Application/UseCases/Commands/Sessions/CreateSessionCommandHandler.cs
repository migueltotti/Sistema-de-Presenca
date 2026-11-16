using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public class CreateSessionCommandHandler(
    IUserContextProvider userContextProvider,
    ISessionRepository sessionRepository,
    ISubjectRepository subjectRepository,
    IRoomRepository roomRepository,
    ILogger<CreateSessionCommandHandler> logger) : ICommandHandler<CreateSessionCommand, Result<CreateSessionResponse>>
{
    public async Task<Result<CreateSessionResponse>> HandleAsync(CreateSessionCommand command, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetOneAsync(x => 
            x.Id == command.Request.SubjectId, 
            includes: ["Professors", "Students"],
            cancellationToken: cancellationToken);

        if (subject is null)
        {
            logger.LogError("Subject with id {SubjectId} - not found", command.Request.SubjectId);
            return Result<CreateSessionResponse>.Failure(SubjectErrors.NotFound);
        }

        if (!(subject.Students?.Any() ?? false))
        {
            logger.LogError("Subject with id {SubjectId} must have at least one student to create a session.", command.Request.SubjectId);
            return Result<CreateSessionResponse>.Failure(SubjectErrors.StudentsRequiredToCreateSession);
        }

        var room = await roomRepository.GetOneAsync(x => x.Id == command.Request.RoomId, cancellationToken: cancellationToken);
        if (room is null)
        {
            logger.LogError("Room with id {RoomId} - not found", command.Request.RoomId);
            return Result<CreateSessionResponse>.Failure(RoomErrors.NotFound);
        }

        // TODO: Procurar por agendamentos na data e horário da criação da aula e não permitir caso ja exista um agendamento para aquela data.

        // Se o usuário que faz a req for professor, verificar se ele pertence a disciplina que está tentando criar a aula. 
        if (userContextProvider.User.Role is UserRole.Professor)
        {
            var professorId = userContextProvider.User.Id;
            if (!subject.Professors.Any(x => x.Id == professorId))
            {
                logger.LogError("Professor {ProfessorId} does not teach the provided Subject {SubjectId}", professorId, subject.Id);
                return Result<CreateSessionResponse>.Failure(SubjectErrors.ProfessorMismatch);
            }
        }

        var newSession = command.Request.ToEntity(userContextProvider.User.Id);

        newSession.Attendances.AddRange(subject.Students
            .Select(student => new Attendance(newSession.Id, student.Id)));

        // TODO: Criar um agendamento para a data e horário da aula, atribuido à sala.

        await sessionRepository.AddAsync(newSession, cancellationToken);

        logger.LogInformation("Session {SessionId} created successfully by {UserEmail}.", newSession.Id, userContextProvider.User.Email);

        return Result<CreateSessionResponse>.Success(new CreateSessionResponse(newSession.Id));
    }
}
