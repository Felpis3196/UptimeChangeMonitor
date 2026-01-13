using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Domain.Entities;

public class ChangeDetection
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public ChangeType ChangeType { get; set; }
    public string? PreviousContentHash { get; set; }
    public string? CurrentContentHash { get; set; }
    public string? ChangeDescription { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual Monitor Monitor { get; set; } = null!;
}
