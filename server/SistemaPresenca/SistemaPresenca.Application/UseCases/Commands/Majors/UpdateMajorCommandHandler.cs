using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Majors;

public class UpdateMajorCommandHandler(
    IMajorRepoitory majorRepoitory,
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

        var majorResponse = major.ToResponse();

        command.PatchRequest
            .Document
            .ApplyTo(majorResponse);

        var updatedMajor = majorResponse.ToEntity();

        var majorWithSameCode = await majorRepoitory.GetOneAsync(x =>
            x.Code == updatedMajor.Code &&
            x.Id != updatedMajor.Id,
            cancellationToken);

        if (majorWithSameCode is not null)
        {
            logger.LogError("Major with code {Code} already exists.", updatedMajor.Code);
            return Result.Failure(MajorErrors.CodeAlreadyExists);
        }

        await majorRepoitory.UpdateAsync(updatedMajor, cancellationToken);

        logger.LogInformation("Major with Id {Id} updated successfully.", updatedMajor.Id);

        return Result.Success();
    }
}
