namespace SistemaPresenca.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int TotalClasses { get; set; }
    public Guid MajorId { get; set; }
    public Major Major { get; set; }
    public Guid? ProfessorId { get; set; }
    public User? Professor { get; set; }
    public IEnumerable<Session> Sessions { get; set; }

    private Subject() : base()
    {
    }

    public Subject(string name, string code, int totalClasses, Guid majorId, Guid? createdByAdminId) : base(createdByAdminId)
    {
        Name = name;
        Code = code;
        TotalClasses = totalClasses;
        MajorId = majorId;
    }
}
