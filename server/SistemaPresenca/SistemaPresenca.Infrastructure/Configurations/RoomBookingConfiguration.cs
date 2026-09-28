using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class RoomBookingConfiguration : BaseEntityConfiguration<RoomBooking>
{
    public override void Configure(EntityTypeBuilder<RoomBooking> builder)
    {
        base.Configure(builder);

        builder.ToTable("RoomBookings");

        builder.Property(b => b.StartsAt)
            .IsRequired();

        builder.Property(b => b.EndsAt)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired();

        builder.HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Professor)
            .WithMany()
            .HasForeignKey(b => b.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Subject)
            .WithMany()
            .HasForeignKey(b => b.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.RoomId, b.StartsAt, b.EndsAt });
    }
}