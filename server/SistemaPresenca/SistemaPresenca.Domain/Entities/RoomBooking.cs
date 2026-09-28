using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Domain.Entities;

public class RoomBooking : BaseEntity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    public Guid ProfessorId { get; set; }
    public User Professor { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public RoomBookingStatus Status { get; set; }

    private RoomBooking() : base()
    {
    }

    public RoomBooking(Guid roomId, Guid professorId, Guid subjectId, DateTime startsAt, DateTime endsAt, Guid? createdByAdminId) : base(createdByAdminId)
    {
        RoomId = roomId;
        Room = default!;
        ProfessorId = professorId;
        Professor = default!;
        SubjectId = subjectId;
        Subject = default!;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Status = RoomBookingStatus.Scheduled;
    }
}