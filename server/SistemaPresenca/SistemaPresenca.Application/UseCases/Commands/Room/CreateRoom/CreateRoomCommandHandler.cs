using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.UseCases.Commands.Rooms;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Room.CreateRoom;

public sealed class CreateRoomCommandHandler(
    IUserContextProvider userContextProvider,
    IRoomRepository roomRepository,
    ILogger<CreateRoomCommandHandler> logger) : ICommandHandler<CreateRoomCommand, Result>
{
    public async Task<Result> HandleAsync(CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var roomWithSameName = await roomRepository.GetOneAsync(x => x.Name == command.Request.Name, cancellationToken: cancellationToken);

        if (roomWithSameName is not null)
        {
            logger.LogError("Room with name {Name} already exists.", command.Request.Name);
            return Result.Failure(RoomErrors.NameAlreadyExists);
        }

        var roomWithSameMicrocontrollerId = await roomRepository.GetOneAsync(
            x => x.MicrocontrollerId == command.Request.MicrocontrollerId, cancellationToken: cancellationToken);

        if (roomWithSameMicrocontrollerId is not null)
        {
            logger.LogError("Room with microcontroller id {MicrocontrollerId} already exists.", command.Request.MicrocontrollerId);
            return Result.Failure(RoomErrors.MicrocontrollerIdAlreadyExists);
        }

        var newRoom = command.Request.ToEntity(userContextProvider.User.Id);

        await roomRepository.AddAsync(newRoom, cancellationToken);

        logger.LogInformation("Room with id {Id} created successfully.", newRoom.Id);

        return Result.Success();
    }
}