using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Entities;
using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Workers.ChangeDetectionWorker;

public class ChangeDetectionProcessor
{
    private readonly IChangeDetectionRepository _changeDetectionRepository;
    private readonly IMonitorRepository _monitorRepository;
    private readonly HttpClient _httpClient;

    public ChangeDetectionProcessor(
        IChangeDetectionRepository changeDetectionRepository,
        IMonitorRepository monitorRepository)
    {
        _changeDetectionRepository = changeDetectionRepository;
        _monitorRepository = monitorRepository;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task ProcessDetectionAsync(ChangeDetectionMessage message)
    {
        var monitor = await _monitorRepository.GetByIdAsync(message.MonitorId);
        if (monitor == null)
        {
            Console.WriteLine($"Monitor {message.MonitorId} not found");
            return;
        }

        try
        {
            var response = await _httpClient.GetStringAsync(message.Url);
            var currentHash = ComputeHash(response);

            var latestDetection = await _changeDetectionRepository.GetLatestByMonitorIdAsync(message.MonitorId);
            
            if (latestDetection == null)
            {
                // First check - just store the hash
                var detection = new ChangeDetection
                {
                    Id = Guid.NewGuid(),
                    MonitorId = message.MonitorId,
                    ChangeType = ChangeType.ContentChanged,
                    CurrentContentHash = currentHash,
                    DetectedAt = DateTime.UtcNow,
                    ChangeDescription = "Initial content snapshot"
                };
                await _changeDetectionRepository.AddAsync(detection);
                Console.WriteLine($"Initial content hash stored for {message.Url}");
            }
            else if (latestDetection.CurrentContentHash != currentHash)
            {
                // Change detected
                var detection = new ChangeDetection
                {
                    Id = Guid.NewGuid(),
                    MonitorId = message.MonitorId,
                    ChangeType = ChangeType.ContentChanged,
                    PreviousContentHash = latestDetection.CurrentContentHash,
                    CurrentContentHash = currentHash,
                    DetectedAt = DateTime.UtcNow,
                    ChangeDescription = "Content change detected"
                };
                await _changeDetectionRepository.AddAsync(detection);
                Console.WriteLine($"Change detected for {message.Url}");
            }
            else
            {
                Console.WriteLine($"No changes detected for {message.Url}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing change detection for {message.Url}: {ex.Message}");
        }
    }

    private string ComputeHash(string content)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
