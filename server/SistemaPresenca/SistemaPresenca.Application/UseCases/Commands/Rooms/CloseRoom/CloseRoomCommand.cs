using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.CloseRoom;

public sealed record CloseRoomCommand(Guid RoomId, Guid UserId) : ICommand<Result>;