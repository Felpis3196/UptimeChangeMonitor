using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class MonitorStatusDto
{
    public Guid MonitorId { get; set; }
    public string MonitorName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public UptimeStatus? CurrentStatus { get; set; }
    public string? StatusDescription { get; set; }
    public int? LastResponseTimeMs { get; set; }
    public string? LastResponseTimeFormatted { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public string? LastCheckedAtFormatted { get; set; }
    public string? TimeSinceLastCheck { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastChangeDetectedAt { get; set; }
    public string? LastChangeDetectedAtFormatted { get; set; }
    public string? TimeSinceLastChange { get; set; }
    public bool HasRecentChanges { get; set; }
}
