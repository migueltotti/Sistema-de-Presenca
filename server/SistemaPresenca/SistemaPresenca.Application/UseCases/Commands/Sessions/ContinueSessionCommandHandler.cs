using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed class ContinueSessionCommandHandler(
    ISessionRepository sessionRepository,
    IUserRepository userRepository,
    ILogger<ContinueSessionCommandHandler> logger) : ICommandHandler<ContinueSessionCommand, Result>
{
    public async Task<Result> HandleAsync(ContinueSessionCommand command, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.GetAsync(x => x.Id == command.SessionId, cancellationToken);
        if (session is null)
        {
            logger.LogError("Session with id {SessionId} - not found", command.SessionId);
            return Result.Failure(SessionErrors.NotFound);
        }

        var professor = await userRepository.GetAsync(x => x.TagId == command.ProfessorTagId && x.Role == UserRole.Professor, cancellationToken);
        if (professor is null)
        {
            logger.LogError("Professor with tagId {ProfessorTagId} - not found", command.ProfessorTagId);
            return Result.Failure(UserErrors.ProfessorNotFound);
        }

        if (!session.ProfessorId.Equals(professor.Id))
        {
            logger.LogError("Professor with tagId {ProfessorTagId} is not assigned to session {SessionId}", command.ProfessorTagId, command.SessionId);
            return Result.Failure(SessionErrors.ProfessorMismatch);
        }

        if (session.FinalizedAt is not null)
        {
            logger.LogError("Session with id {SessionId} is already finished", command.SessionId);
            return Result.Failure(SessionErrors.AlreadyFinished);
        }

        return Result.Success();
    }
}
