using UptimeChangeMonitor.Application.DTOs;

namespace UptimeChangeMonitor.Application.Interfaces;

public interface IUptimeCheckService
{
    Task<IEnumerable<UptimeCheckDto>> GetHistoryByMonitorIdAsync(Guid monitorId, int? limit = null);
    Task<UptimeCheckDto?> GetLatestByMonitorIdAsync(Guid monitorId);
}
