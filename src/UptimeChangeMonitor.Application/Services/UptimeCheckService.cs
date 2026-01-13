using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;
using UptimeChangeMonitor.Domain.Enums;

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
        var dtos = checks.Select(check => 
        {
            var dto = _mapper.Map<UptimeCheckDto>(check);
            EnrichUptimeCheckDto(dto);
            return dto;
        }).ToList();
        
        return dtos;
    }

    public async Task<UptimeCheckDto?> GetLatestByMonitorIdAsync(Guid monitorId)
    {
        var check = await _uptimeCheckRepository.GetLatestByMonitorIdAsync(monitorId);
        if (check == null)
            return null;
            
        var dto = _mapper.Map<UptimeCheckDto>(check);
        EnrichUptimeCheckDto(dto);
        return dto;
    }
    
    private static void EnrichUptimeCheckDto(UptimeCheckDto dto)
    {
        // Sempre preencher todas as propriedades formatadas
        dto.StatusDescription = GetUptimeStatusDescription(dto.Status);
        dto.IsOnline = dto.Status == UptimeStatus.Online;
        dto.ResponseTimeFormatted = FormatResponseTime(dto.ResponseTimeMs);
        dto.StatusCodeDescription = GetStatusCodeDescription(dto.StatusCode);
        dto.CheckedAtFormatted = dto.CheckedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
        dto.TimeAgo = GetTimeAgo(dto.CheckedAt);
    }
    
    private static string GetUptimeStatusDescription(UptimeStatus status)
    {
        return status switch
        {
            UptimeStatus.Online => "Online",
            UptimeStatus.Offline => "Offline",
            UptimeStatus.Timeout => "Timeout",
            UptimeStatus.Error => "Erro",
            _ => "Desconhecido"
        };
    }
    
    private static string? FormatResponseTime(int? responseTimeMs)
    {
        if (!responseTimeMs.HasValue)
            return null;

        if (responseTimeMs < 1000)
            return $"{responseTimeMs}ms";
        
        var seconds = responseTimeMs.Value / 1000.0;
        return $"{seconds:F2}s";
    }
    
    private static string? GetStatusCodeDescription(int? statusCode)
    {
        if (!statusCode.HasValue)
            return null;

        return statusCode.Value switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            301 => "Moved Permanently",
            302 => "Found",
            304 => "Not Modified",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            500 => "Internal Server Error",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            _ => $"HTTP {statusCode}"
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
