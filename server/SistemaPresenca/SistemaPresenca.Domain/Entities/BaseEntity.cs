namespace SistemaPresenca.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public Guid? DeletedByAdminId { get; set; }
    public User? DeletedByAdmin { get; set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid? createdById)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        CreatedById = createdById;
    }
}
