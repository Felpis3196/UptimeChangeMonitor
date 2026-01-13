using UptimeChangeMonitor.Application.DTOs;

namespace UptimeChangeMonitor.Application.Interfaces;

public interface IChangeDetectionService
{
    Task<IEnumerable<ChangeDetectionDto>> GetHistoryByMonitorIdAsync(Guid monitorId, int? limit = null);
    Task<ChangeDetectionDto?> GetLatestByMonitorIdAsync(Guid monitorId);
}
