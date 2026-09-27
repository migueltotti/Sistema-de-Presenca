using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms;

public sealed record DeleteRoomCommand(Guid Id) : ICommand<Result>;
