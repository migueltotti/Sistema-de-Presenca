namespace SistemaPresenca.Application.Responses.Rooms;

public sealed record GetRoomResponse(
    Guid Id,
    string Name,
    string Location,
    string MicrocontrollerId,
    RoomStatus Status
);