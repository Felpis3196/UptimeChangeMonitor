using Microsoft.EntityFrameworkCore;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Entities;
using UptimeChangeMonitor.Infrastructure.Data;

namespace UptimeChangeMonitor.Infrastructure.Repositories;

public class ChangeDetectionRepository : Repository<ChangeDetection>, IChangeDetectionRepository
{
    public ChangeDetectionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChangeDetection>> GetByMonitorIdAsync(Guid monitorId, int? limit = null)
    {
        var query = _dbSet
            .Where(cd => cd.MonitorId == monitorId)
            .OrderByDescending(cd => cd.DetectedAt);

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<ChangeDetection>)query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<ChangeDetection?> GetLatestByMonitorIdAsync(Guid monitorId)
    {
        return await _dbSet
            .Where(cd => cd.MonitorId == monitorId)
            .OrderByDescending(cd => cd.DetectedAt)
            .FirstOrDefaultAsync();
    }
}
