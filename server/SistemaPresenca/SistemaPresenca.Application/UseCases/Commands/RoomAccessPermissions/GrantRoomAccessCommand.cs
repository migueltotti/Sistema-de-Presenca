using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;

public sealed record GrantRoomAccessCommand(Guid UserId, Guid RoomId, Guid? GrantedByAdminId) : ICommand<Result>;