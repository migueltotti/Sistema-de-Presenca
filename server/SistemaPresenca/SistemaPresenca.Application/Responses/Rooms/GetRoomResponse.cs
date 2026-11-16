using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Responses.Rooms;

public sealed record GetRoomResponse(
    Guid Id,
    string Name,
    string Location,
    string MicrocontrollerId,
    RoomStatus Status,
    IEnumerable<GetRoomAccessResponse> AccessPermissions
);

public sealed record GetRoomAccessResponse(
    Guid UserId,
    DateTime GrantedAt,
    bool Active
);