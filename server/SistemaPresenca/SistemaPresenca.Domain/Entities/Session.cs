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
    public Guid ProfessorId { get; set; }
    public User Professor { get; set; } // TODO: Remover essa propriedade e utilizar a lista de professores dentro de Subject
    public List<Attendance> Attendances { get; set; }

    private Session() : base()
    {
    }

    public Session(DateTime initiedAt, int totalLessons, Guid subjectId, Guid roomId, Guid professorId, Guid? createdByAdminId) : base(createdByAdminId)
    {
        InitiedAt = initiedAt;
        TotalLessons = totalLessons;
        SubjectId = subjectId;
        RoomId = roomId;
        ProfessorId = professorId;
        Attendances = [];
        Subject = default!;
        Room = default!; 
        Professor = default!;
    }
}
