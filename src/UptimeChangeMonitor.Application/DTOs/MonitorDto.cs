using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class MonitorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public MonitorStatus Status { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; }
    public string CheckIntervalFormatted { get; set; } = string.Empty;
    public bool MonitorUptime { get; set; }
    public bool MonitorChanges { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedAtFormatted { get; set; } = string.Empty;
    public string CreatedAtTimeAgo { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedAtFormatted { get; set; } = string.Empty;
    public string UpdatedAtTimeAgo { get; set; } = string.Empty;
    public DateTime? LastCheckedAt { get; set; }
    public string? LastCheckedAtFormatted { get; set; }
    public string? LastCheckedAtTimeAgo { get; set; }
}
