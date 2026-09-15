using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Majors;

public sealed class CreateMajorsCommandHandler(
    IMajorRepository majorRepository,
    ILogger<CreateMajorsCommandHandler> logger) : ICommandHandler<CreateMajorsCommand, Result>
{
    public async Task<Result> HandleAsync(CreateMajorsCommand command, CancellationToken cancellationToken = default)
    {
        var existingMajor = await majorRepository.GetOneAsync(x => x.Code == command.Request.Code, cancellationToken);

        if (existingMajor is not null)
        {
            logger.LogWarning("Major with code {Code} already exists.", command.Request.Code);
            return Result.Failure(MajorErrors.CodeAlreadyExists);
        }

        var newMajor = command.Request.ToEntity();

        await majorRepository.AddAsync(newMajor, cancellationToken);

        logger.LogInformation("Major with code {Code} created successfully.", newMajor.Code);

        return Result.Success();
    }
}
