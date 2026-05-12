using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class MajorConfiguration : BaseEntityConfiguration<Major>
{
    public override void Configure(EntityTypeBuilder<Major> builder)
    {
        base.Configure(builder);

        builder.ToTable("Majors");

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(m => m.Code)
            .IsUnique();
    }
}
