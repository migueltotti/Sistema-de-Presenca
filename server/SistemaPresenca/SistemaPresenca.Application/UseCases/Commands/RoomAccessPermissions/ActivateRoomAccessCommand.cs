using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed record ActivateRoomAccessCommand(Guid RoomId, Guid PermissionId) : ICommand<Result>;