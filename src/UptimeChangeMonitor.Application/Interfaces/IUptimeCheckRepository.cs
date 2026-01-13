using UptimeChangeMonitor.Domain.Entities;

namespace UptimeChangeMonitor.Application.Interfaces;

public interface IUptimeCheckRepository : IRepository<UptimeCheck>
{
    Task<IEnumerable<UptimeCheck>> GetByMonitorIdAsync(Guid monitorId, int? limit = null);
    Task<UptimeCheck?> GetLatestByMonitorIdAsync(Guid monitorId);
}
