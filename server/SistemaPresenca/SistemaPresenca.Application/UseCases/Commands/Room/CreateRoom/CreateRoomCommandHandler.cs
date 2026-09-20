using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms;

public sealed class CreateRoomCommandHandler(
    IRoomRepository roomRepository,
    ILogger<CreateRoomCommandHandler> logger) : ICommandHandler<CreateRoomCommand, Result>
{
    public async Task<Result> HandleAsync(CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var roomWithSameName = await roomRepository.GetOneAsync(x => x.Name == command.Request.Name, cancellationToken);

        if (roomWithSameName is not null)
        {
            logger.LogError("Room with name {Name} already exists.", command.Request.Name);
            return Result.Failure(RoomErrors.NameAlreadyExists);
        }

        var roomWithSameMicrocontrollerId = await roomRepository.GetOneAsync(
            x => x.MicrocontrollerId == command.Request.MicrocontrollerId, cancellationToken);

        if (roomWithSameMicrocontrollerId is not null)
        {
            logger.LogError("Room with microcontroller id {MicrocontrollerId} already exists.", command.Request.MicrocontrollerId);
            return Result.Failure(RoomErrors.MicrocontrollerIdAlreadyExists);
        }

        var newRoom = command.Request.ToEntity();

        await roomRepository.AddAsync(newRoom, cancellationToken);

        logger.LogInformation("Room with id {Id} created successfully.", newRoom.Id);

        return Result.Success();
    }
}