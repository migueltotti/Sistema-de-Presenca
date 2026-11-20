using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed class AddSubjectMemberCommandHandler(
    ISubjectRepository subjectRepository,
    IUserRepository userRepository,
    ILogger<AddSubjectMemberCommandHandler> logger) : ICommandHandler<AddSubjectMemberCommand, Result>
{
    public async Task<Result> HandleAsync(AddSubjectMemberCommand command, CancellationToken cancellationToken = default)
    {
        var subject = await subjectRepository.GetOneAsync(
            x => x.Id == command.SubjectId,
            includes: ["Professors", "Students"],
            cancellationToken: cancellationToken);

        if (subject is null)
        {
            logger.LogError("Subject with id {SubjectId} not found.", command.SubjectId);
            return Result.Failure(SubjectErrors.NotFound);
        }

        var user = await userRepository.GetOneAsync(x => x.Id == command.MemberId, cancellationToken: cancellationToken);

        if (user is null)
        {
            logger.LogError("User with id {UserId} not found.", command.MemberId);
            return Result.Failure(UserErrors.NotFound);
        }

        var targetCollection = (command.MemberRole == MemberRoleRequest.Professor
            ? subject.Professors
            : subject.Students);

        if (targetCollection.Any(x => x.Id == user.Id))
        {
            logger.LogError("{UserType} with id {UserId} already exists in Subject with id {SubjectId}.", command.MemberRole.ToString(), user.Id,command.SubjectId);
            return Result.Failure(SubjectErrors.UserAlreadyAssigned(command.MemberRole.ToString()));
        }

        targetCollection.Add(user);

        await subjectRepository.UpdateAsync(subject, cancellationToken);

        logger.LogInformation("{UserType} with id {UserId} assigned successfully to Subject with id {SubjectId}.", command.MemberRole.ToString(), user.Id, command.SubjectId);

        return Result.Success();
    }
}
