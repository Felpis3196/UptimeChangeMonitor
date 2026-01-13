using Microsoft.EntityFrameworkCore;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Enums;
using UptimeChangeMonitor.Infrastructure.Data;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Infrastructure.Repositories;

public class MonitorRepository : Repository<MonitorEntity>, IMonitorRepository
{
    public MonitorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MonitorEntity?> GetByIdWithChecksAsync(Guid id)
    {
        return await _dbSet
            .Include(m => m.UptimeChecks.OrderByDescending(uc => uc.CheckedAt).Take(10))
            .Include(m => m.ChangeDetections.OrderByDescending(cd => cd.DetectedAt).Take(10))
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MonitorEntity>> GetActiveMonitorsAsync()
    {
        return await _dbSet
            .Where(m => m.Status == MonitorStatus.Active)
            .ToListAsync();
    }
}
