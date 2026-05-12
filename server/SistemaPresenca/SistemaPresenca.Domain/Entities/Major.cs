namespace SistemaPresenca.Domain.Entities;

public class Major : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public IEnumerable<Subject>? Subjects { get; set; }

    private Major() : base()
    {
    }

    public Major(string name, string code, Guid? createdByAdminId) : base(createdByAdminId)
    {
        Name = name;
        Code = code;
    }
}
