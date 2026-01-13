using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Application.DTOs;

public class UptimeCheckDto
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public UptimeStatus Status { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public int? ResponseTimeMs { get; set; }
    public string? ResponseTimeFormatted { get; set; }
    public int? StatusCode { get; set; }
    public string? StatusCodeDescription { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
    public string CheckedAtFormatted { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
}
