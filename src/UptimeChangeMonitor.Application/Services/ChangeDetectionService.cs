using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Enums;

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
        var dtos = detections.Select(detection => 
        {
            var dto = _mapper.Map<ChangeDetectionDto>(detection);
            EnrichChangeDetectionDto(dto);
            return dto;
        }).ToList();
        
        return dtos;
    }

    public async Task<ChangeDetectionDto?> GetLatestByMonitorIdAsync(Guid monitorId)
    {
        var detection = await _changeDetectionRepository.GetLatestByMonitorIdAsync(monitorId);
        if (detection == null)
            return null;
            
        var dto = _mapper.Map<ChangeDetectionDto>(detection);
        EnrichChangeDetectionDto(dto);
        return dto;
    }
    
    private static void EnrichChangeDetectionDto(ChangeDetectionDto dto)
    {
        // Sempre preencher todas as propriedades formatadas
        dto.ChangeTypeDescription = GetChangeTypeDescription(dto.ChangeType);
        dto.HasSignificantChange = !string.IsNullOrEmpty(dto.ChangeDescription);
        dto.DetectedAtFormatted = dto.DetectedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
        dto.TimeAgo = GetTimeAgo(dto.DetectedAt);
    }
    
    private static string GetChangeTypeDescription(ChangeType changeType)
    {
        return changeType switch
        {
            ChangeType.ContentChanged => "Conteúdo Alterado",
            ChangeType.StructureChanged => "Estrutura Alterada",
            ChangeType.StatusChanged => "Status Alterado",
            _ => "Desconhecido"
        };
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
