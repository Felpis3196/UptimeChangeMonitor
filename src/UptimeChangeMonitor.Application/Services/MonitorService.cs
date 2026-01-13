using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Enums;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Application.Services;

public class MonitorService : IMonitorService
{
    private readonly IMonitorRepository _monitorRepository;
    private readonly IUptimeCheckRepository _uptimeCheckRepository;
    private readonly IChangeDetectionRepository _changeDetectionRepository;
    private readonly IQueueService _queueService;
    private readonly IMapper _mapper;

    public MonitorService(
        IMonitorRepository monitorRepository,
        IUptimeCheckRepository uptimeCheckRepository,
        IChangeDetectionRepository changeDetectionRepository,
        IQueueService queueService,
        IMapper mapper)
    {
        _monitorRepository = monitorRepository;
        _uptimeCheckRepository = uptimeCheckRepository;
        _changeDetectionRepository = changeDetectionRepository;
        _queueService = queueService;
        _mapper = mapper;
    }

    public async Task<MonitorDto?> GetByIdAsync(Guid id)
    {
        var monitor = await _monitorRepository.GetByIdAsync(id);
        return monitor == null ? null : _mapper.Map<MonitorDto>(monitor);
    }

    public async Task<IEnumerable<MonitorDto>> GetAllAsync()
    {
        var monitors = await _monitorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<MonitorDto>>(monitors);
    }

    public async Task<MonitorDto> CreateAsync(CreateMonitorDto createDto)
    {
        var monitor = _mapper.Map<MonitorEntity>(createDto);
        monitor.Id = Guid.NewGuid();
        monitor.CreatedAt = DateTime.UtcNow;
        monitor.UpdatedAt = DateTime.UtcNow;
        monitor.Status = MonitorStatus.Active;

        var createdMonitor = await _monitorRepository.AddAsync(monitor);
        
        // Publish job to queue if monitoring is enabled
        if (createdMonitor.MonitorUptime)
        {
            _queueService.PublishUptimeCheckJob(createdMonitor.Id, createdMonitor.Url);
        }
        if (createdMonitor.MonitorChanges)
        {
            _queueService.PublishChangeDetectionJob(createdMonitor.Id, createdMonitor.Url);
        }
        
        return _mapper.Map<MonitorDto>(createdMonitor);
    }

    public async Task<MonitorDto?> UpdateAsync(Guid id, UpdateMonitorDto updateDto)
    {
        var monitor = await _monitorRepository.GetByIdAsync(id);
        if (monitor == null)
            return null;

        _mapper.Map(updateDto, monitor);
        monitor.UpdatedAt = DateTime.UtcNow;

        await _monitorRepository.UpdateAsync(monitor);
        return _mapper.Map<MonitorDto>(monitor);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var monitor = await _monitorRepository.GetByIdAsync(id);
        if (monitor == null)
            return false;

        await _monitorRepository.DeleteAsync(monitor);
        return true;
    }

    public async Task<MonitorStatusDto?> GetStatusAsync(Guid id)
    {
        var monitor = await _monitorRepository.GetByIdAsync(id);
        if (monitor == null)
            return null;

        var latestUptimeCheck = await _uptimeCheckRepository.GetLatestByMonitorIdAsync(id);
        var latestChange = await _changeDetectionRepository.GetLatestByMonitorIdAsync(id);

        var statusDto = new MonitorStatusDto
        {
            MonitorId = monitor.Id,
            MonitorName = monitor.Name,
            Url = monitor.Url,
            CurrentStatus = latestUptimeCheck?.Status,
            LastResponseTimeMs = latestUptimeCheck?.ResponseTimeMs,
            LastCheckedAt = latestUptimeCheck?.CheckedAt,
            IsOnline = latestUptimeCheck?.Status == UptimeStatus.Online,
            LastChangeDetectedAt = latestChange?.DetectedAt
        };

        // Preencher propriedades descritivas
        FillStatusDescriptions(statusDto, latestUptimeCheck, latestChange);

        return statusDto;
    }

    public async Task<IEnumerable<MonitorStatusDto>> GetAllStatusAsync()
    {
        var monitors = await _monitorRepository.GetAllAsync();
        var statusList = new List<MonitorStatusDto>();

        foreach (var monitor in monitors)
        {
            var latestUptimeCheck = await _uptimeCheckRepository.GetLatestByMonitorIdAsync(monitor.Id);
            var latestChange = await _changeDetectionRepository.GetLatestByMonitorIdAsync(monitor.Id);

            var statusDto = new MonitorStatusDto
            {
                MonitorId = monitor.Id,
                MonitorName = monitor.Name,
                Url = monitor.Url,
                CurrentStatus = latestUptimeCheck?.Status,
                LastResponseTimeMs = latestUptimeCheck?.ResponseTimeMs,
                LastCheckedAt = latestUptimeCheck?.CheckedAt,
                IsOnline = latestUptimeCheck?.Status == UptimeStatus.Online,
                LastChangeDetectedAt = latestChange?.DetectedAt
            };

            // Preencher propriedades descritivas
            FillStatusDescriptions(statusDto, latestUptimeCheck, latestChange);

            statusList.Add(statusDto);
        }

        return statusList;
    }

    private static void FillStatusDescriptions(
        MonitorStatusDto statusDto,
        Domain.Entities.UptimeCheck? latestUptimeCheck,
        Domain.Entities.ChangeDetection? latestChange)
    {
        // Status description
        if (statusDto.CurrentStatus.HasValue)
        {
            statusDto.StatusDescription = statusDto.CurrentStatus.Value switch
            {
                UptimeStatus.Online => "Online",
                UptimeStatus.Offline => "Offline",
                UptimeStatus.Timeout => "Timeout",
                UptimeStatus.Error => "Erro",
                _ => "Desconhecido"
            };
        }

        // Response time formatted
        if (statusDto.LastResponseTimeMs.HasValue)
        {
            var rt = statusDto.LastResponseTimeMs.Value;
            statusDto.LastResponseTimeFormatted = rt < 1000 ? $"{rt}ms" : $"{rt / 1000.0:F2}s";
        }

        // Last checked at formatted
        if (statusDto.LastCheckedAt.HasValue)
        {
            statusDto.LastCheckedAtFormatted = statusDto.LastCheckedAt.Value.ToString("yyyy-MM-dd HH:mm:ss UTC");
            statusDto.TimeSinceLastCheck = GetTimeAgo(statusDto.LastCheckedAt.Value);
        }

        // Last change detected at formatted
        if (statusDto.LastChangeDetectedAt.HasValue)
        {
            statusDto.LastChangeDetectedAtFormatted = statusDto.LastChangeDetectedAt.Value.ToString("yyyy-MM-dd HH:mm:ss UTC");
            statusDto.TimeSinceLastChange = GetTimeAgo(statusDto.LastChangeDetectedAt.Value);
            
            // Consider recent if within last 24 hours
            var timeSinceChange = DateTime.UtcNow - statusDto.LastChangeDetectedAt.Value;
            statusDto.HasRecentChanges = timeSinceChange.TotalHours < 24;
        }
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return $"há {(int)timeSpan.TotalSeconds} segundo{(timeSpan.TotalSeconds != 1 ? "s" : "")}";
        
        if (timeSpan.TotalMinutes < 60)
            return $"há {(int)timeSpan.TotalMinutes} minuto{(timeSpan.TotalMinutes != 1 ? "s" : "")}";
        
        if (timeSpan.TotalHours < 24)
            return $"há {(int)timeSpan.TotalHours} hora{(timeSpan.TotalHours != 1 ? "s" : "")}";
        
        if (timeSpan.TotalDays < 30)
            return $"há {(int)timeSpan.TotalDays} dia{(timeSpan.TotalDays != 1 ? "s" : "")}";
        
        if (timeSpan.TotalDays < 365)
        {
            var months = (int)(timeSpan.TotalDays / 30);
            return $"há {months} mês{(months != 1 ? "es" : "")}";
        }
        
        var years = (int)(timeSpan.TotalDays / 365);
        return $"há {years} ano{(years != 1 ? "s" : "")}";
    }
}
