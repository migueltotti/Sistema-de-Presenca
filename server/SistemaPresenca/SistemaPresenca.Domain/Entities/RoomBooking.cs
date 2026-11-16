using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Domain.Entities;

public class RoomBooking : BaseEntity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    public Guid ProfessorId { get; set; }
    public User Professor { get; set; }
    public Guid? SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public RoomBookingType Type { get; set; }
    public bool AutoStartAttendance { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public RoomBookingStatus Status { get; set; }
    public Session? Session { get; set; }

    private RoomBooking() : base()
    {
    }

    public RoomBooking(Guid roomId, Guid professorId, Guid? subjectId, RoomBookingType type, bool autoStartAttendance, DateTime startsAt, DateTime endsAt, Guid? createdById) : base(createdById)
    {
        RoomId = roomId;
        Room = default!;
        ProfessorId = professorId;
        Professor = default!;
        SubjectId = subjectId;
        Subject = default!;
        Type = type;
        AutoStartAttendance = autoStartAttendance;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Status = RoomBookingStatus.Scheduled;
    }
}