using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Rooms;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Queries.Rooms;

public sealed class GetRoomsQueryHandler(IRoomRepository roomRepository) : IQueryHandler<GetRoomsQuery, IEnumerable<GetRoomResponse>>
{
    public async Task<IEnumerable<GetRoomResponse>> HandleAsync(GetRoomsQuery message, CancellationToken cancellationToken = default)
    {
        var filter = new RoomFilters.Builder()
            .WithIds(message.Request.Ids)
            .WithNames(message.Request.Names)
            .WithLocations(message.Request.Locations)
            .Build();

        var rooms = await roomRepository.GetRoomsAsync(filter, cancellationToken);

        return rooms.Select(x => x.ToResponse());
    }
}