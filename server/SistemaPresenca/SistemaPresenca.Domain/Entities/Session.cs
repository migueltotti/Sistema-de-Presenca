namespace SistemaPresenca.Domain.Entities;

public class Session : BaseEntity
{
    public DateTime InitiedAt { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public int TotalLessons { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    public List<Attendance> Attendances { get; set; }

    private Session() : base()
    {
    }

    public Session(DateTime initiedAt, int totalLessons, Guid subjectId, Guid roomId, Guid? createdById) : base(createdById)
    {
        InitiedAt = initiedAt;
        TotalLessons = totalLessons;
        SubjectId = subjectId;
        RoomId = roomId;
        Attendances = [];
        Subject = default!;
        Room = default!;
    }
}
