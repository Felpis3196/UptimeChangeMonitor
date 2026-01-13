using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class MonitorStatusDto
{
    public Guid MonitorId { get; set; }
    public string MonitorName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public UptimeStatus? CurrentStatus { get; set; }
    public int? LastResponseTimeMs { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastChangeDetectedAt { get; set; }
}
