using UptimeChangeMonitor.Domain.Entities;

namespace UptimeChangeMonitor.Application.Interfaces;

public interface IChangeDetectionRepository : IRepository<ChangeDetection>
{
    Task<IEnumerable<ChangeDetection>> GetByMonitorIdAsync(Guid monitorId, int? limit = null);
    Task<ChangeDetection?> GetLatestByMonitorIdAsync(Guid monitorId);
}
