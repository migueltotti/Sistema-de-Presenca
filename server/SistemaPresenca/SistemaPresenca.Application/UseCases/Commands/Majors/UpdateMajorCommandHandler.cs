using FluentValidation;
using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Majors;

public class UpdateMajorCommandHandler(
    IMajorRepository majorRepoitory,
    IValidator<UpdateMajorRequest> validator,
    ILogger<UpdateMajorCommandHandler> logger) : ICommandHandler<UpdateMajorCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateMajorCommand command, CancellationToken cancellationToken = default)
    {
        var major = await majorRepoitory.GetOneAsync(x => x.Id == command.Id, cancellationToken);

        if (major is null)
        {
            logger.LogError("Major with id {Id} not found.", command.Id);
            return Result.Failure(MajorErrors.NotFound);
        }

        var majorUpdateRequest = major.ToUpdateRequest();

        command.
            PatchRequest
            .ApplyTo(majorUpdateRequest);

        var validationResult = validator.Validate(majorUpdateRequest);
        if (!validationResult.IsValid)
        {
            logger.LogError("Invalid major update request for major with id {Id}.", command.Id);
            return Result.Failure(MajorErrors.InvalidUpdateRequest(string.Join(",", validationResult.Errors)));
        }

        majorUpdateRequest.UpdateEntity(major);

        var majorWithSameCode = await majorRepoitory.GetOneAsync(x =>
            x.Code == majorUpdateRequest.Code &&
            x.Id != major.Id,
            cancellationToken);

        if (majorWithSameCode is not null)
        {
            logger.LogError("Major with code {Code} already exists.", majorUpdateRequest.Code);
            return Result.Failure(MajorErrors.CodeAlreadyExists);
        }

        await majorRepoitory.UpdateAsync(major, cancellationToken);

        logger.LogInformation("Major with Id {Id} updated successfully.", major.Id);

        return Result.Success();
    }
}
