namespace UptimeChangeMonitor.Application.Interfaces;

public interface IQueueService
{
    void PublishUptimeCheckJob(Guid monitorId, string url);
    void PublishChangeDetectionJob(Guid monitorId, string url);
}
