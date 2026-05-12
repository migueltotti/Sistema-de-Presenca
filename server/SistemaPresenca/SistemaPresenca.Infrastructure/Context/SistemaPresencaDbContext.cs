using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Infrastructure.Context;

public class SistemaPresencaDbContext : DbContext
{
    public SistemaPresencaDbContext(DbContextOptions<SistemaPresencaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemaPresencaDbContext).Assembly);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Major> Majors { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
}
