namespace SistemaPresenca.Domain.Entities;

public class Room : BaseEntity
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string MicrocontrollerId { get; set; }
    public RoomStatus Status { get; set; }
    public List<Session> Sessions { get; set; }

    private Room() : base()
    {
    }

    public Room(string name, string location, string microcontrollerId, Guid? createdByAdminId) : base(createdByAdminId)
    {
        Name = name;
        Location = location;
        MicrocontrollerId = microcontrollerId; // decidir qual abordagem vamos usar para nomear o microcontrolador
        Status = RoomStatus.Active;
        Sessions = [];
    }
}