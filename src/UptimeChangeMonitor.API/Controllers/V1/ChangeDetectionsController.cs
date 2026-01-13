using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;

namespace UptimeChangeMonitor.API.Controllers.V1;

/// <summary>
/// Controller para histórico de detecções de mudança (v1)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/monitors/{monitorId}/[controller]")]
[Produces("application/json")]
public class ChangeDetectionsController : ControllerBase
{
    private readonly IChangeDetectionService _changeDetectionService;

    public ChangeDetectionsController(IChangeDetectionService changeDetectionService)
    {
        _changeDetectionService = changeDetectionService;
    }

    /// <summary>
    /// Obtém o histórico de detecções de mudança de um monitor
    /// </summary>
    /// <param name="monitorId">ID do monitor</param>
    /// <param name="limit">Limite de registros a retornar (opcional, padrão: sem limite)</param>
    /// <returns>Histórico de detecções com estatísticas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetHistory(
        Guid monitorId,
        [FromQuery] int? limit = null)
    {
        var detections = await _changeDetectionService.GetHistoryByMonitorIdAsync(monitorId, limit);
        var detectionsList = detections.ToList();
        
        // Calcular estatísticas
        var total = detectionsList.Count;
        var contentChanged = detectionsList.Count(d => d.ChangeType == Domain.Enums.ChangeType.ContentChanged);
        var structureChanged = detectionsList.Count(d => d.ChangeType == Domain.Enums.ChangeType.StructureChanged);
        var statusChanged = detectionsList.Count(d => d.ChangeType == Domain.Enums.ChangeType.StatusChanged);
        var significantChanges = detectionsList.Count(d => d.HasSignificantChange);
        
        var lastChange = detectionsList.OrderByDescending(d => d.DetectedAt).FirstOrDefault();
        var firstChange = detectionsList.OrderBy(d => d.DetectedAt).FirstOrDefault();
        
        var timeSinceLastChange = lastChange != null 
            ? DateTime.UtcNow - lastChange.DetectedAt 
            : (TimeSpan?)null;

        return Ok(new
        {
            monitorId,
            summary = new
            {
                total,
                contentChanged,
                structureChanged,
                statusChanged,
                significantChanges,
                lastChangeAt = lastChange?.DetectedAt,
                firstChangeAt = firstChange?.DetectedAt,
                timeSinceLastChange = timeSinceLastChange.HasValue 
                    ? GetTimeAgoFormatted(timeSinceLastChange.Value) 
                    : null,
                changeFrequency = total > 0 && timeSinceLastChange.HasValue && timeSinceLastChange.Value.TotalDays > 0
                    ? $"{(total / timeSinceLastChange.Value.TotalDays):F2} mudanças por dia"
                    : null
            },
            detections = detectionsList,
            retrievedAt = DateTime.UtcNow
        });
    }
    
    private static string GetTimeAgoFormatted(TimeSpan timeSpan)
    {
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

    /// <summary>
    /// Obtém a última detecção de mudança de um monitor
    /// </summary>
    /// <param name="monitorId">ID do monitor</param>
    /// <returns>Última detecção com informações adicionais</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetLatest(Guid monitorId)
    {
        var detection = await _changeDetectionService.GetLatestByMonitorIdAsync(monitorId);
        if (detection == null)
            return NotFound(new { 
                monitorId, 
                message = "Nenhuma detecção de mudança encontrada para este monitor",
                suggestion = "O monitor pode não ter mudanças detectadas ainda ou não existe"
            });

        // Verificar se há mudanças recentes (últimas 24 horas)
        var timeSinceDetection = DateTime.UtcNow - detection.DetectedAt;
        var isRecent = timeSinceDetection.TotalHours < 24;

        return Ok(new
        {
            detection,
            metadata = new
            {
                isRecent,
                timeSinceDetection = GetTimeAgoFormatted(timeSinceDetection),
                recentChangeWarning = isRecent 
                    ? "Mudança detectada recentemente" 
                    : $"Última mudança detectada há {Math.Round(timeSinceDetection.TotalDays, 1)} dias",
                retrievedAt = DateTime.UtcNow
            }
        });
    }
}
