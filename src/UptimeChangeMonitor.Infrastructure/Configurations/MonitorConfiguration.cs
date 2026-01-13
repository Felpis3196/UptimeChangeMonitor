using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Infrastructure.Configurations;

public class MonitorConfiguration : IEntityTypeConfiguration<MonitorEntity>
{
    public void Configure(EntityTypeBuilder<MonitorEntity> builder)
    {
        builder.ToTable("Monitors");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.CheckIntervalSeconds)
            .IsRequired()
            .HasDefaultValue(60);

        builder.Property(m => m.MonitorUptime)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.MonitorChanges)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany<UptimeChangeMonitor.Domain.Entities.UptimeCheck>(m => m.UptimeChecks)
            .WithOne(uc => uc.Monitor)
            .HasForeignKey(uc => uc.MonitorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<UptimeChangeMonitor.Domain.Entities.ChangeDetection>(m => m.ChangeDetections)
            .WithOne(cd => cd.Monitor)
            .HasForeignKey(cd => cd.MonitorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.Url);
    }
}
