using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Application.Responses.Rooms;

namespace SistemaPresenca.Application.UseCases.Queries.Rooms;

public sealed record GetRoomsQuery(GetRoomsRequest Request) : IQuery<IEnumerable<GetRoomResponse>>;