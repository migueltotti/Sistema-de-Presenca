namespace SistemaPresenca.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public Guid? CreatedByAdminId { get; set; }
    public User? CreatedByAdmin { get; set; }
    public Guid? DeletedByAdminId { get; set; }
    public User? DeletedByAdmin { get; set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid? createdByAdminId)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        CreatedByAdminId = createdByAdminId;
    }
}
