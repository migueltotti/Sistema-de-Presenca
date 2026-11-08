using LiteBus.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms;

public sealed class DeleteRoomCommandHandler(
    IUserContextProvider userContextProvider,
    IRoomRepository roomRepository,
	ILogger<DeleteRoomCommandHandler> logger) : ICommandHandler<DeleteRoomCommand, Result>
{
	public async Task<Result> HandleAsync(DeleteRoomCommand command, CancellationToken cancellationToken = default)
	{
		var room = await roomRepository.GetOneAsync(x => x.Id == command.Id, cancellationToken);

		if (room is null)
		{
			logger.LogError("Room with id {Id} not found.", command.Id);
			return Result.Failure(RoomErrors.NotFound);
		}

		await roomRepository.DeleteAsync(room, userContextProvider.UserId, cancellationToken);

		logger.LogInformation("Room with id {Id} deleted successfully.", command.Id);

		return Result.Success();
	}
}
