namespace SistemaPresenca.Domain.Entities;

public class RoomAccessPermission : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    public DateTime GrantedAt { get; set; }
    public bool Active { get; set; }

    private RoomAccessPermission() : base()
    {
    }

    public RoomAccessPermission(Guid userId, Guid roomId, Guid? createdByAdminId) : base(createdByAdminId)
    {
        UserId = userId;
        RoomId = roomId;
        GrantedAt = DateTime.UtcNow;
        Active = true;
    }
}