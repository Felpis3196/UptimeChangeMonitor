using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Infrastructure.Data;
using UptimeChangeMonitor.Infrastructure.Repositories;
using UptimeChangeMonitor.Workers.Shared;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Workers.ChangeDetectionWorker;

public class ChangeDetectionWorker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IModel? _channel;

    public ChangeDetectionWorker(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMQ();

        var queueName = _configuration["RabbitMQ:QueueNames:ChangeDetection"] ?? "change_detection_queue";
        var handler = new ChangeDetectionMessageHandler(_channel!, queueName, _serviceProvider);
        handler.StartConsuming();

        Console.WriteLine($"Change Detection Worker started. Listening to queue: {queueName}");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task InitializeRabbitMQ()
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
        _channel = _connection.CreateModel();
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}

public class ChangeDetectionMessageHandler : MessageHandler
{
    private readonly IServiceProvider _serviceProvider;

    public ChangeDetectionMessageHandler(IModel channel, string queueName, IServiceProvider serviceProvider)
        : base(channel, queueName)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ProcessMessage(string message)
    {
        var detectionMessage = JsonSerializer.Deserialize<ChangeDetectionMessage>(message);
        if (detectionMessage == null)
        {
            throw new InvalidOperationException("Invalid message format");
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var changeDetectionRepository = new ChangeDetectionRepository(dbContext);
        var monitorRepository = new MonitorRepository(dbContext);

        var processor = new ChangeDetectionProcessor(changeDetectionRepository, monitorRepository);
        await processor.ProcessDetectionAsync(detectionMessage);
    }
}
