using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Room;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms.CloseRoom;

public record CloseRoomByTagIdCommand(string MicrocontrollerId, string TagId, CloseRoomSessionRequest SessionRequest) : ICommand<Result>;