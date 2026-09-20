namespace SistemaPresenca.Application.Requests.Rooms;

public sealed record CreateRoomRequest(
    string Name,
    string Location,
    string MicrocontrollerId,
    Guid? CreatedByAdminId
);