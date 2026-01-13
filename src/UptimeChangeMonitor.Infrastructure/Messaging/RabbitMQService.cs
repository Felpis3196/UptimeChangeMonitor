using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace UptimeChangeMonitor.Infrastructure.Messaging;

public class RabbitMQService : IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMQService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IModel GetChannel()
    {
        if (_channel != null && _channel.IsOpen)
            return _channel;

        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/"
            };

            _connection = factory.CreateConnection();
        }

        _channel = _connection.CreateModel();
        return _channel;
    }

    public void DeclareQueue(string queueName)
    {
        var channel = GetChannel();
        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
