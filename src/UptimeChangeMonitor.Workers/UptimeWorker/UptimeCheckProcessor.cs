using System.Net.Http;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Entities;
using UptimeChangeMonitor.Domain.Enums;

namespace UptimeChangeMonitor.Workers.UptimeWorker;

public class UptimeCheckProcessor
{
    private readonly IUptimeCheckRepository _uptimeCheckRepository;
    private readonly IMonitorRepository _monitorRepository;
    private readonly HttpClient _httpClient;

    public UptimeCheckProcessor(
        IUptimeCheckRepository uptimeCheckRepository,
        IMonitorRepository monitorRepository)
    {
        _uptimeCheckRepository = uptimeCheckRepository;
        _monitorRepository = monitorRepository;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task ProcessCheckAsync(UptimeCheckMessage message)
    {
        var monitor = await _monitorRepository.GetByIdAsync(message.MonitorId);
        if (monitor == null)
        {
            Console.WriteLine($"Monitor {message.MonitorId} not found");
            return;
        }

        var check = new UptimeCheck
        {
            Id = Guid.NewGuid(),
            MonitorId = message.MonitorId,
            CheckedAt = DateTime.UtcNow
        };

        try
        {
            var startTime = DateTime.UtcNow;
            var response = await _httpClient.GetAsync(message.Url);
            var endTime = DateTime.UtcNow;
            var responseTime = (int)(endTime - startTime).TotalMilliseconds;

            check.Status = response.IsSuccessStatusCode ? UptimeStatus.Online : UptimeStatus.Offline;
            check.StatusCode = (int)response.StatusCode;
            check.ResponseTimeMs = responseTime;
        }
        catch (TaskCanceledException)
        {
            check.Status = UptimeStatus.Timeout;
            check.ErrorMessage = "Request timeout";
        }
        catch (HttpRequestException ex)
        {
            check.Status = UptimeStatus.Error;
            check.ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            check.Status = UptimeStatus.Error;
            check.ErrorMessage = ex.Message;
        }

        await _uptimeCheckRepository.AddAsync(check);

        // Update monitor's last checked time
        monitor.LastCheckedAt = DateTime.UtcNow;
        await _monitorRepository.UpdateAsync(monitor);

        Console.WriteLine($"Uptime check completed for {message.Url}: {check.Status}");
    }
}
