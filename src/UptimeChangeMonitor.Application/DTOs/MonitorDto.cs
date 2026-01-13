using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class MonitorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public MonitorStatus Status { get; set; }
    public int CheckIntervalSeconds { get; set; }
    public bool MonitorUptime { get; set; }
    public bool MonitorChanges { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastCheckedAt { get; set; }
}
