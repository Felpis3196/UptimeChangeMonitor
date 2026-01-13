using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;

namespace UptimeChangeMonitor.Application.Services;

public class UptimeCheckService : IUptimeCheckService
{
    private readonly IUptimeCheckRepository _uptimeCheckRepository;
    private readonly IMapper _mapper;

    public UptimeCheckService(IUptimeCheckRepository uptimeCheckRepository, IMapper mapper)
    {
        _uptimeCheckRepository = uptimeCheckRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UptimeCheckDto>> GetHistoryByMonitorIdAsync(Guid monitorId, int? limit = null)
    {
        var checks = await _uptimeCheckRepository.GetByMonitorIdAsync(monitorId, limit);
        return _mapper.Map<IEnumerable<UptimeCheckDto>>(checks);
    }

    public async Task<UptimeCheckDto?> GetLatestByMonitorIdAsync(Guid monitorId)
    {
        var check = await _uptimeCheckRepository.GetLatestByMonitorIdAsync(monitorId);
        return check == null ? null : _mapper.Map<UptimeCheckDto>(check);
    }
}
