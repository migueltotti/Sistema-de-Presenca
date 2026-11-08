using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Application.Responses.Rooms;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class RoomMapper
{
    public static Room ToEntity(this CreateRoomRequest request, Guid createdByAdminId)
    {
        return new Room(
            request.Name,
            request.Location,
            request.MicrocontrollerId,
            createdByAdminId
        );
    }

    public static GetRoomResponse ToResponse(this Room room)
    {
        return new GetRoomResponse(
            room.Id,
            room.Name,
            room.Location,
            room.MicrocontrollerId,
            room.Status
        );
    }

    public static UpdateRoomRequest ToUpdateRequest(this Room room)
    {
        return new UpdateRoomRequest(
            room.Name,
            room.Location,
            room.MicrocontrollerId
        );
    }

    public static void UpdatedEntity(this UpdateRoomRequest request, Room room)
    {
        room.Name = request.Name;
        room.Location = request.Location;
        room.MicrocontrollerId = request.MicrocontrollerId;
    }
}