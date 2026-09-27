using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class RoomConfiguration : BaseEntityConfiguration<Room>
{
    public override void Configure(EntityTypeBuilder<Room> builder)
    {
        base.Configure(builder);

        builder.ToTable("Rooms");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.MicrocontrollerId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Status)
            .IsRequired();

        builder.HasMany(r => r.Sessions)
            .WithOne(s => s.Room)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");

        builder.HasIndex(r => r.MicrocontrollerId)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}