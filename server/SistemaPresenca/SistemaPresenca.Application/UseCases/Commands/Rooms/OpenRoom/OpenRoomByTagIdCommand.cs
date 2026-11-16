using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.OpenRoom;

public record OpenRoomByTagIdCommand(
    string MicrocontrollerId,
    string TagId,
    OpenRoomSessionRequest SessionRequest) : ICommand<Result>;