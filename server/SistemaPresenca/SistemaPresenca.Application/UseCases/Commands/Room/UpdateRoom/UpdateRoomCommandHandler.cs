using FluentValidation;
using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms;

public sealed class UpdateRoomCommandHandler(
	IRoomRepository roomRepository,
	IValidator<UpdateRoomRequest> validator,
	ILogger<UpdateRoomCommandHandler> logger) : ICommandHandler<UpdateRoomCommand, Result>
{
	public async Task<Result> HandleAsync(UpdateRoomCommand command, CancellationToken cancellationToken = default)
	{
		var room = await roomRepository.GetOneAsync(x => x.Id == command.Id, cancellationToken);

		if (room is null)
		{
			logger.LogError("Room with id {Id} not found.", command.Id);
			return Result.Failure(RoomErrors.NotFound);
		}

		var roomUpdateRequest = room.ToUpdateRequest();

		command.PatchRequest.ApplyTo(roomUpdateRequest);

		var validationResult = validator.Validate(roomUpdateRequest);
		if (!validationResult.IsValid)
		{
			logger.LogError("Invalid room update request for room with id {Id}.", command.Id);
			return Result.Failure(RoomErrors.InvalidUpdateRequest(string.Join(",", validationResult.Errors)));
		}

		roomUpdateRequest.UpdatedEntity(room);

		var roomWithSameName = await roomRepository.GetOneAsync(
			x => x.Name == roomUpdateRequest.Name && x.Id != command.Id, cancellationToken);

		if (roomWithSameName is not null)
		{
			logger.LogError("Room with name {Name} already exists.", roomUpdateRequest.Name);
			return Result.Failure(RoomErrors.NameAlreadyExists);
		}

		var roomWithSameMicrocontrollerId = await roomRepository.GetOneAsync(
			x => x.MicrocontrollerId == roomUpdateRequest.MicrocontrollerId && x.Id != command.Id, cancellationToken);

		if (roomWithSameMicrocontrollerId is not null)
		{
			logger.LogError("Room with microcontroller id {MicrocontrollerId} already exists.", roomUpdateRequest.MicrocontrollerId);
			return Result.Failure(RoomErrors.MicrocontrollerIdAlreadyExists);
		}

		await roomRepository.UpdateAsync(room, cancellationToken);

		logger.LogInformation("Room with id {Id} updated successfully.", command.Id);

		return Result.Success();
	}
}
