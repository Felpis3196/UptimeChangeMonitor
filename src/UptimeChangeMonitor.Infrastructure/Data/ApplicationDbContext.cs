using Microsoft.EntityFrameworkCore;
using UptimeChangeMonitor.Domain.Entities;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MonitorEntity> Monitors { get; set; }
    public DbSet<UptimeCheck> UptimeChecks { get; set; }
    public DbSet<ChangeDetection> ChangeDetections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
