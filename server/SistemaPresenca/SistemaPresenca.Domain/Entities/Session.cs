namespace SistemaPresenca.Domain.Entities;

public class Session : BaseEntity
{
    public DateTime InitiedAt { get; set; }
    public DateTime? FinalizedAt { get; set; }
    public int TotalLessons { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public Guid ProfessorId { get; set; }
    public User Professor { get; set; }
    public IEnumerable<Attendance> Attendances { get; set; }

    private Session() : base()
    {
    }

    public Session(DateTime initiedAt, int totalLessons, Guid subjectId, Guid professorId, Guid? createdByAdminId) : base(createdByAdminId)
    {
        InitiedAt = initiedAt;
        TotalLessons = totalLessons;
        SubjectId = subjectId;
        ProfessorId = professorId;
    }
}
