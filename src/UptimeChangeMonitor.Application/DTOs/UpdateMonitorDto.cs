namespace UptimeChangeMonitor.Application.DTOs;

public class UpdateMonitorDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; }
    public bool MonitorUptime { get; set; }
    public bool MonitorChanges { get; set; }
}
