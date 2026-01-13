using Microsoft.EntityFrameworkCore;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Entities;
using UptimeChangeMonitor.Infrastructure.Data;

namespace UptimeChangeMonitor.Infrastructure.Repositories;

public class UptimeCheckRepository : Repository<UptimeCheck>, IUptimeCheckRepository
{
    public UptimeCheckRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UptimeCheck>> GetByMonitorIdAsync(Guid monitorId, int? limit = null)
    {
        var query = _dbSet
            .Where(uc => uc.MonitorId == monitorId)
            .OrderByDescending(uc => uc.CheckedAt);

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<UptimeCheck>)query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<UptimeCheck?> GetLatestByMonitorIdAsync(Guid monitorId)
    {
        return await _dbSet
            .Where(uc => uc.MonitorId == monitorId)
            .OrderByDescending(uc => uc.CheckedAt)
            .FirstOrDefaultAsync();
    }
}
