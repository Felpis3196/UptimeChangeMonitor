using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Infrastructure.Messaging;

namespace UptimeChangeMonitor.Infrastructure.Services;

public class QueueService : IQueueService
{
    private readonly MessagePublisher _messagePublisher;

    public QueueService(MessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public void PublishUptimeCheckJob(Guid monitorId, string url)
    {
        _messagePublisher.PublishUptimeCheckMessage(monitorId, url);
    }

    public void PublishChangeDetectionJob(Guid monitorId, string url)
    {
        _messagePublisher.PublishChangeDetectionMessage(monitorId, url);
    }
}
