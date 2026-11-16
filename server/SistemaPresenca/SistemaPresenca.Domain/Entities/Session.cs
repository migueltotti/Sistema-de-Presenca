using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Domain.Entities;

public class Session : BaseEntity
{
    public DateOnly Date { get; set; }
    public TimeOnly ScheduledStartTime { get; set; }
    public TimeOnly ScheduledEndTime { get; set; }
    public Guid? StartedById { get; set; }
    public User? StartedBy { get; set; }
    public DateTime? InitiedAt { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public SessionStatus Status { get; set; }
    public int TotalLessons { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
    public List<Attendance> Attendances { get; set; }
    public Guid? RoomBookingId { get; set; }
    public RoomBooking? RoomBooking { get; set; }

    private Session() : base()
    {
    }

    public Session(DateOnly date, TimeOnly scheduledStartTime, TimeOnly scheduledEndTime, int totalLessons, Guid subjectId, Guid roomId, Guid? createdById, Guid? roomBookingId = null) : base(createdById)
    {
        Date = date;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
        TotalLessons = totalLessons;
        SubjectId = subjectId;
        RoomId = roomId;
        Status = SessionStatus.Scheduled;
        RoomBookingId = roomBookingId;
        Attendances = [];

        Subject = default!;
    }

    public void Finalize()
    {
        FinalizedAt = DateTime.UtcNow;
        Status = SessionStatus.Completed;
    }
}
