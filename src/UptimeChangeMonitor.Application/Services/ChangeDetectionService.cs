using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;

namespace UptimeChangeMonitor.Application.Services;

public class ChangeDetectionService : IChangeDetectionService
{
    private readonly IChangeDetectionRepository _changeDetectionRepository;
    private readonly IMapper _mapper;

    public ChangeDetectionService(IChangeDetectionRepository changeDetectionRepository, IMapper mapper)
    {
        _changeDetectionRepository = changeDetectionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChangeDetectionDto>> GetHistoryByMonitorIdAsync(Guid monitorId, int? limit = null)
    {
        var detections = await _changeDetectionRepository.GetByMonitorIdAsync(monitorId, limit);
        return _mapper.Map<IEnumerable<ChangeDetectionDto>>(detections);
    }

    public async Task<ChangeDetectionDto?> GetLatestByMonitorIdAsync(Guid monitorId)
    {
        var detection = await _changeDetectionRepository.GetLatestByMonitorIdAsync(monitorId);
        return detection == null ? null : _mapper.Map<ChangeDetectionDto>(detection);
    }
}
