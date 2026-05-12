using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class SessionConfiguration : BaseEntityConfiguration<Session>
{
    public override void Configure(EntityTypeBuilder<Session> builder)
    {
        base.Configure(builder);

        builder.ToTable("Sessions");

        builder.Property(s => s.InitiedAt)
            .IsRequired();

        builder.Property(s => s.FinalizedAt)
            .IsRequired(false);

        builder.Property(s => s.TotalLessons)
            .IsRequired(false);

        
        builder.HasOne(s => s.Subject)
            .WithMany(sub => sub.Sessions)
            .HasForeignKey(s => s.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Professor)
            .WithMany()
            .HasForeignKey(s => s.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
