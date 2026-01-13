namespace UptimeChangeMonitor.Application.DTOs;

public class CreateMonitorDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; } = 60;
    public bool MonitorUptime { get; set; } = true;
    public bool MonitorChanges { get; set; } = false;
}
