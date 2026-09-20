using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms;

public sealed record CreateRoomCommand(CreateRoomRequest Request) : ICommand<Result>;