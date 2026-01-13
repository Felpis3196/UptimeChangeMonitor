using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UptimeChangeMonitor.Domain.Entities;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Infrastructure.Configurations;

public class UptimeCheckConfiguration : IEntityTypeConfiguration<UptimeCheck>
{
    public void Configure(EntityTypeBuilder<UptimeCheck> builder)
    {
        builder.ToTable("UptimeChecks");

        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.MonitorId)
            .IsRequired();

        builder.Property(uc => uc.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(uc => uc.CheckedAt)
            .IsRequired();

        builder.Property(uc => uc.ErrorMessage)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne<MonitorEntity>(uc => uc.Monitor)
            .WithMany(m => m.UptimeChecks)
            .HasForeignKey(uc => uc.MonitorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(uc => uc.MonitorId);
        builder.HasIndex(uc => uc.CheckedAt);
        builder.HasIndex(uc => new { uc.MonitorId, uc.CheckedAt });
    }
}
