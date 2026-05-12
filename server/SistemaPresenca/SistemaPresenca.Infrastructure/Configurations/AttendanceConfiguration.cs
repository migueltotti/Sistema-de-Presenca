using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => new {a.StudentId, a.SessionId});


        builder.Property(a => a.RegisteredAt)
            .IsRequired(false);

        builder.HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Session)
            .WithMany()
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
