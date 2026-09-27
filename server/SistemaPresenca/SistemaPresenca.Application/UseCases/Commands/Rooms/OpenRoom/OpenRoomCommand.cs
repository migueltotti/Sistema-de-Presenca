using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.OpenRoom;

public sealed record OpenRoomCommand(Guid RoomId, Guid UserId) : ICommand<Result>;