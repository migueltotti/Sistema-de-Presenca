using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class RoomAccessPermissionConfiguration : BaseEntityConfiguration<RoomAccessPermission>
{
    public override void Configure(EntityTypeBuilder<RoomAccessPermission> builder)
    {
        base.Configure(builder);

        builder.ToTable("RoomAccessPermissions");

        builder.Property(permission => permission.UserId)
            .IsRequired();

        builder.Property(permission => permission.RoomId)
            .IsRequired();

        builder.Property(permission => permission.GrantedAt)
            .IsRequired();

        builder.Property(permission => permission.Active)
            .IsRequired();

        builder.HasOne(permission => permission.User)
            .WithMany(user => user.RoomAccessPermissions)
            .HasForeignKey(permission => permission.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(permission => permission.Room)
            .WithMany(room => room.RoomAccessPermissions)
            .HasForeignKey(permission => permission.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(permission => new { permission.UserId, permission.RoomId })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}