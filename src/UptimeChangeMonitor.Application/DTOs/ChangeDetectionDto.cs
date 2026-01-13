using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class ChangeDetectionDto
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public ChangeType ChangeType { get; set; }
    public string ChangeTypeDescription { get; set; } = string.Empty;
    public string? PreviousContentHash { get; set; }
    public string? CurrentContentHash { get; set; }
    public string? ChangeDescription { get; set; }
    public bool HasSignificantChange { get; set; }
    public DateTime DetectedAt { get; set; }
    public string DetectedAtFormatted { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
}
