namespace SistemaPresenca.Domain.Entities;

public class Attendance
{
    public Guid SessionId { get; set; }
    public Session Session { get; set; }
    public Guid StudentId { get; set; }
    public User Student { get; set; }
    public DateTime? RegisteredAt { get; set; }

    private Attendance()
    {
    }

    public Attendance(Guid sessionId, Guid studentId)
    {
        SessionId = sessionId;
        StudentId = studentId;
    }
}
