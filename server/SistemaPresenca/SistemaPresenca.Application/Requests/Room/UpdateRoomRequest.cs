namespace SistemaPresenca.Application.Requests.Rooms;

public sealed record UpdateRoomRequest(
    string Name,
    string Location,
    string MicrocontrollerId
);