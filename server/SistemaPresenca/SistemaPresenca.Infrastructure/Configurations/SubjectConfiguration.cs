using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class SubjectConfiguration : BaseEntityConfiguration<Subject>
{
    public override void Configure(EntityTypeBuilder<Subject> builder)
    {
        base.Configure(builder);

        builder.ToTable("Subjects");


        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.TotalClasses)
            .IsRequired();

        
        builder.HasOne(s => s.Major)
            .WithMany(s => s.Subjects)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Professor)
            .WithMany()
            .HasForeignKey(s => s.ProfessorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.Code)
            .IsUnique();
    }
}
