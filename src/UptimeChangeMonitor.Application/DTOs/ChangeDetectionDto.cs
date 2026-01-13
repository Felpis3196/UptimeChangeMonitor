using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class ChangeDetectionDto
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public ChangeType ChangeType { get; set; }
    public string? PreviousContentHash { get; set; }
    public string? CurrentContentHash { get; set; }
    public string? ChangeDescription { get; set; }
    public DateTime DetectedAt { get; set; }
}
