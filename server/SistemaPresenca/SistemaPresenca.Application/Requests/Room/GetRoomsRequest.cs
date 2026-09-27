namespace SistemaPresenca.Application.Requests.Rooms;

public sealed class GetRoomsRequest
{
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Locations { get; set; }
}