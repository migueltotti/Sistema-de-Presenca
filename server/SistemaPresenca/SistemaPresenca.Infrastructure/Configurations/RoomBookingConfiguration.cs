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

        builder.Property(b => b.SubjectId)
            .IsRequired(false);

        builder.Property(b => b.Type)
            .IsRequired();

        builder.Property(b => b.AutoStartAttendance)
            .IsRequired();

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
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Session)
            .WithOne(s => s.RoomBooking)
            .HasForeignKey<Session>(s => s.RoomBookingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(b => new { b.RoomId, b.StartsAt, b.EndsAt });
    }
}