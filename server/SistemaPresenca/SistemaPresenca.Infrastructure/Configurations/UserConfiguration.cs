using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");


        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Password)
            .HasMaxLength(200);

        builder.Property(u => u.RegistrationId)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.Cpf)
            .HasMaxLength(11);

        builder.Property(u => u.TagId)
            .HasMaxLength(50);

        builder.Property(u => u.Role)
            .HasConversion<string>();


        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Cpf)
            .IsUnique();

        builder.HasIndex(u => u.RegistrationId)
            .IsUnique();

        builder.HasIndex(u => u.TagId)
            .IsUnique();
    }
}
