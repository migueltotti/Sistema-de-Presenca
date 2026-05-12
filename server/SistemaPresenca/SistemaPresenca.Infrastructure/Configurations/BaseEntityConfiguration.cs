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

        builder.Property(x => x.CreatedByAdminId)
            .IsRequired(false);

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}
