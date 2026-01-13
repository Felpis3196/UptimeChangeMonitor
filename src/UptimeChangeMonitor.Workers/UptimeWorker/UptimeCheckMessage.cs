namespace UptimeChangeMonitor.Workers.UptimeWorker;

public class UptimeCheckMessage
{
    public Guid MonitorId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
