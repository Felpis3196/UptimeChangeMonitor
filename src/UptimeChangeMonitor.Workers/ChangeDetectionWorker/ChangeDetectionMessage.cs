namespace UptimeChangeMonitor.Workers.ChangeDetectionWorker;

public class ChangeDetectionMessage
{
    public Guid MonitorId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
