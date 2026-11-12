using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed class EndSessionCommandHandler(
    IUserContextProvider userContextProvider,
    ISessionRepository sessionRepository,
    IUserRepository userRepository,
    IMqttProducer mqttProducer,
    ILogger<EndSessionCommandHandler> logger) : ICommandHandler<EndSessionCommand, Result>
{
    public async Task<Result> HandleAsync(EndSessionCommand command, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.GetOneAsync(
            x => x.Id == command.SessionId,
            includes: ["Subject", "Room"],
            cancellationToken: cancellationToken);

        if (session is null)
        {
            logger.LogError("Session with id {SessionId} - not found", command.SessionId);
            return Result.Failure(SessionErrors.NotFound);
        }

        var userId = userContextProvider.User.Id;

        if (userContextProvider.User.Role == UserRole.Professor)
        {
            var professor = await userRepository.GetOneAsync(x => x.Id == userId && x.Role == UserRole.Professor, cancellationToken: cancellationToken);
            if (professor is null)
            {
                logger.LogError("Professor with id {UserId} - not found", userId);
                return Result.Failure(UserErrors.ProfessorNotFound);
            }

            if (!session.Subject.ProfessorIds.Contains(userId))
            {
                logger.LogError("Professor with id {UserId} is not assigned to session {SessionId}", userId, command.SessionId);
                return Result.Failure(SessionErrors.ProfessorMismatch);
            }
        }

        session.FinalizedAt = DateTime.UtcNow;

        await sessionRepository.UpdateAsync(session, cancellationToken);

        await mqttProducer.PublishAsync(session.Room.MicrocontrollerId, session.ToEndSessionEvent(), cancellationToken);

        logger.LogInformation("Session with id {SessionId} - finalized successfully", command.SessionId);

        return Result.Success();
    }
}
