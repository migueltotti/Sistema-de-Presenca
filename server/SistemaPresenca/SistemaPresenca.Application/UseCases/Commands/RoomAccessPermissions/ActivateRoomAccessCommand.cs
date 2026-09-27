using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed record ActivateRoomAccessCommand(Guid Id) : ICommand<Result>;