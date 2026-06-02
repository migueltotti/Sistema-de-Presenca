using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed class RegisterAttendanceCommandHandler(
    ISessionRepository sessionRepository,
    IUserRepository userRepository,
    ILogger<RegisterAttendanceCommandHandler> logger) : ICommandHandler<RegisterAttendanceCommand, Result>
{
    public async Task<Result> HandleAsync(RegisterAttendanceCommand command, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.GetWithAttendancesAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            logger.LogError("Session with id {SessionId} - not found", command.SessionId);
            return Result.Failure(SessionErrors.NotFound);
        }

        if (session.FinalizedAt is not null)
        {
            logger.LogError("Session with id {SessionId} is already finished", command.SessionId);
            return Result.Failure(SessionErrors.AlreadyFinished);
        }

        var student = await userRepository.GetAsync(x => x.TagId == command.StudentTagId && x.Role == UserRole.Student, cancellationToken);
        if (student is null)
        {
            logger.LogError("Student with tagId {StudentTagId} - not found", command.StudentTagId);
            return Result.Failure(UserErrors.StudentNotFound);
        }

        var studentAttendance = session.Attendances.FirstOrDefault(x => x.StudentId == student.Id);
        if (studentAttendance is null)
        {
            logger.LogError("Student with tagId {StudentTagId} - not found in session {SessionId}", command.StudentTagId, command.SessionId);
            return Result.Failure(SessionErrors.StudentNotFound);
        }

        if (studentAttendance.RegisteredAt is not null)
        {
            logger.LogError("Student with tagId {StudentTagId} already has attendance registered for session {SessionId}", command.StudentTagId, command.SessionId);
            return Result.Failure(SessionErrors.StudentAttendanceAlreadyRegistered);
        }

        studentAttendance.RegisteredAt = DateTime.UtcNow;

        await sessionRepository.UpdateAsync(session, cancellationToken);

        logger.LogInformation("Attendance registered for student with tagId {StudentTagId} in session {SessionId}", command.StudentTagId, command.SessionId);

        return Result.Success();
    }
}
