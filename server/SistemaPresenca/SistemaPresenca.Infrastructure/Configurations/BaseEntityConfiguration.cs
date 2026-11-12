using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedByAdminId)
            .IsRequired(false);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(x => x.DeletedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.DeletedByAdminId)
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}
