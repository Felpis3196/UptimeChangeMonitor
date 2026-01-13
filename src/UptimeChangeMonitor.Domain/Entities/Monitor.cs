using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Domain.Entities;

public class Monitor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public MonitorStatus Status { get; set; } = MonitorStatus.Active;
    public int CheckIntervalSeconds { get; set; } = 60;
    public bool MonitorUptime { get; set; } = true;
    public bool MonitorChanges { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastCheckedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<UptimeCheck> UptimeChecks { get; set; } = new List<UptimeCheck>();
    public virtual ICollection<ChangeDetection> ChangeDetections { get; set; } = new List<ChangeDetection>();
}
