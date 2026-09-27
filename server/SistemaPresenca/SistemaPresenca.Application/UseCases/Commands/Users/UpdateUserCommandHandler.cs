using FluentValidation;
using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<UpdateUserRequest> validator,
    ILogger<UpdateUserCommandHandler> logger) : ICommandHandler<UpdateUserCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetOneAsync(x => x.Id == command.Id, cancellationToken);

        if (user is null)
        {
            logger.LogError("User with id {Id} not found.", command.Id);
            return Result.Failure(UserErrors.NotFound);
        }

        var userUpdateRequest = user.ToUpdateRequest();

        command.
            PatchRequest
            .ApplyTo(userUpdateRequest);

        var validationResult = validator.Validate(userUpdateRequest);
        if (!validationResult.IsValid)
        {
            logger.LogError("Invalid user update request for subject with id {Id}.", command.Id);
            return Result.Failure(UserErrors.InvalidUpdateRequest(string.Join(",", validationResult.Errors)));
        }

        userUpdateRequest.UpdatedEntity(user);

        var subjectWithSameEmailOrTagId = await userRepository.GetOneAsync(x => 
            (x.Email == userUpdateRequest.Email || (x.TagId == userUpdateRequest.TagId && x.TagId != null))
            && x.Id != command.Id, cancellationToken);

        if (subjectWithSameEmailOrTagId is not null)
        {
            logger.LogError("User with Email {Email} or TagId {TagId} already exists.", userUpdateRequest.Email, userUpdateRequest.TagId);
            return Result.Failure(UserErrors.EmailOrTagIdAlreadyExists);
        }

        await userRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("User with id {Id} updated successfully.", command.Id);

        return Result.Success();
    }
}
