using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Domain.Entities;

public class UptimeCheck
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public UptimeStatus Status { get; set; }
    public int? ResponseTimeMs { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual Monitor Monitor { get; set; } = null!;
}
