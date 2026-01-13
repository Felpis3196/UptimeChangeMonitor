using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace UptimeChangeMonitor.Infrastructure.Messaging;

public class MessagePublisher : IDisposable
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly IConfiguration _configuration;

    public MessagePublisher(RabbitMQService rabbitMQService, IConfiguration configuration)
    {
        _rabbitMQService = rabbitMQService;
        _configuration = configuration;
    }

    public void PublishUptimeCheckMessage(Guid monitorId, string url)
    {
        var message = new
        {
            MonitorId = monitorId,
            Url = url,
            Timestamp = DateTime.UtcNow
        };

        PublishMessage(
            queueName: _configuration["RabbitMQ:QueueNames:UptimeCheck"] ?? "uptime_check_queue",
            message: message);
    }

    public void PublishChangeDetectionMessage(Guid monitorId, string url)
    {
        var message = new
        {
            MonitorId = monitorId,
            Url = url,
            Timestamp = DateTime.UtcNow
        };

        PublishMessage(
            queueName: _configuration["RabbitMQ:QueueNames:ChangeDetection"] ?? "change_detection_queue",
            message: message);
    }

    private void PublishMessage(string queueName, object message)
    {
        var channel = _rabbitMQService.GetChannel();
        _rabbitMQService.DeclareQueue(queueName);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body);
    }

    public void Dispose()
    {
        _rabbitMQService.Dispose();
    }
}
