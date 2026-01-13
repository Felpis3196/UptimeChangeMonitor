using UptimeChangeMonitor.Domain.Entities;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Application.Interfaces;

public interface IMonitorRepository : IRepository<MonitorEntity>
{
    Task<MonitorEntity?> GetByIdWithChecksAsync(Guid id);
    Task<IEnumerable<MonitorEntity>> GetActiveMonitorsAsync();
}
