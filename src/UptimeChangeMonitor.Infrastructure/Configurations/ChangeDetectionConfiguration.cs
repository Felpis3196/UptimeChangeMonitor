using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UptimeChangeMonitor.Domain.Entities;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Infrastructure.Configurations;

public class ChangeDetectionConfiguration : IEntityTypeConfiguration<ChangeDetection>
{
    public void Configure(EntityTypeBuilder<ChangeDetection> builder)
    {
        builder.ToTable("ChangeDetections");

        builder.HasKey(cd => cd.Id);

        builder.Property(cd => cd.MonitorId)
            .IsRequired();

        builder.Property(cd => cd.ChangeType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cd => cd.DetectedAt)
            .IsRequired();

        builder.Property(cd => cd.ChangeDescription)
            .HasMaxLength(2000);

        builder.Property(cd => cd.PreviousContentHash)
            .HasMaxLength(64);

        builder.Property(cd => cd.CurrentContentHash)
            .HasMaxLength(64);

        // Relationships
        builder.HasOne<MonitorEntity>(cd => cd.Monitor)
            .WithMany(m => m.ChangeDetections)
            .HasForeignKey(cd => cd.MonitorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(cd => cd.MonitorId);
        builder.HasIndex(cd => cd.DetectedAt);
        builder.HasIndex(cd => new { cd.MonitorId, cd.DetectedAt });
    }
}
