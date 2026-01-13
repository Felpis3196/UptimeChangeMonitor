using AutoMapper;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Domain.Enums;
using MonitorEntity = UptimeChangeMonitor.Domain.Entities.Monitor;

namespace UptimeChangeMonitor.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Monitor mappings with computed properties
        CreateMap<MonitorEntity, MonitorDto>()
            .AfterMap((src, dest) =>
            {
                dest.StatusDescription = GetMonitorStatusDescription(src.Status);
                dest.CheckIntervalFormatted = FormatCheckInterval(src.CheckIntervalSeconds);
                dest.CreatedAtFormatted = src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
                dest.CreatedAtTimeAgo = GetTimeAgo(src.CreatedAt);
                dest.UpdatedAtFormatted = src.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
                dest.UpdatedAtTimeAgo = GetTimeAgo(src.UpdatedAt);
                if (src.LastCheckedAt.HasValue)
                {
                    dest.LastCheckedAtFormatted = src.LastCheckedAt.Value.ToString("yyyy-MM-dd HH:mm:ss UTC");
                    dest.LastCheckedAtTimeAgo = GetTimeAgo(src.LastCheckedAt.Value);
                }
            });
        CreateMap<CreateMonitorDto, MonitorEntity>();
        CreateMap<UpdateMonitorDto, MonitorEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UptimeCheck mappings with computed properties
        CreateMap<UptimeChangeMonitor.Domain.Entities.UptimeCheck, UptimeCheckDto>()
            .AfterMap((src, dest) =>
            {
                dest.StatusDescription = GetUptimeStatusDescription(src.Status);
                dest.IsOnline = src.Status == UptimeStatus.Online;
                dest.ResponseTimeFormatted = FormatResponseTime(src.ResponseTimeMs);
                dest.StatusCodeDescription = GetStatusCodeDescription(src.StatusCode);
                dest.CheckedAtFormatted = src.CheckedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
                dest.TimeAgo = GetTimeAgo(src.CheckedAt);
            });

        // ChangeDetection mappings with computed properties
        CreateMap<UptimeChangeMonitor.Domain.Entities.ChangeDetection, ChangeDetectionDto>()
            .AfterMap((src, dest) =>
            {
                dest.ChangeTypeDescription = GetChangeTypeDescription(src.ChangeType);
                dest.HasSignificantChange = !string.IsNullOrEmpty(src.ChangeDescription);
                dest.DetectedAtFormatted = src.DetectedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
                dest.TimeAgo = GetTimeAgo(src.DetectedAt);
            });
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
    
    private static string GetMonitorStatusDescription(MonitorStatus status)
    {
        return status switch
        {
            MonitorStatus.Active => "Ativo",
            MonitorStatus.Inactive => "Inativo",
            MonitorStatus.Paused => "Pausado",
            _ => "Desconhecido"
        };
    }
    
    private static string FormatCheckInterval(int seconds)
    {
        if (seconds < 60)
            return $"{seconds} segundo{(seconds != 1 ? "s" : "")}";
        
        if (seconds < 3600)
        {
            var minutes = seconds / 60;
            return $"{minutes} minuto{(minutes != 1 ? "s" : "")}";
        }
        
        var hours = seconds / 3600;
        return $"{hours} hora{(hours != 1 ? "s" : "")}";
    }
}
