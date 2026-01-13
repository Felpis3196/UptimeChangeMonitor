using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;

namespace UptimeChangeMonitor.API.Controllers.V1;

/// <summary>
/// Controller para histórico de verificações de uptime (v1)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/monitors/{monitorId}/[controller]")]
[Produces("application/json")]
public class UptimeChecksController : ControllerBase
{
    private readonly IUptimeCheckService _uptimeCheckService;

    public UptimeChecksController(IUptimeCheckService uptimeCheckService)
    {
        _uptimeCheckService = uptimeCheckService;
    }

    /// <summary>
    /// Obtém o histórico de verificações de uptime de um monitor
    /// </summary>
    /// <param name="monitorId">ID do monitor</param>
    /// <param name="limit">Limite de registros a retornar (opcional, padrão: sem limite)</param>
    /// <returns>Histórico de verificações com estatísticas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetHistory(
        Guid monitorId,
        [FromQuery] int? limit = null)
    {
        var checks = await _uptimeCheckService.GetHistoryByMonitorIdAsync(monitorId, limit);
        var checksList = checks.ToList();
        
        // Calcular estatísticas
        var total = checksList.Count;
        var online = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Online);
        var offline = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Offline);
        var timeout = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Timeout);
        var error = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Error);
        
        var avgResponseTime = checksList
            .Where(c => c.ResponseTimeMs.HasValue)
            .Select(c => c.ResponseTimeMs!.Value)
            .DefaultIfEmpty(0)
            .Average();
        
        var uptimePercentage = total > 0 ? (online * 100.0 / total) : 0;

        return Ok(new
        {
            monitorId,
            summary = new
            {
                total,
                online,
                offline,
                timeout,
                error,
                uptimePercentage = Math.Round(uptimePercentage, 2),
                averageResponseTimeMs = avgResponseTime > 0 ? Math.Round(avgResponseTime, 2) : (double?)null,
                averageResponseTimeFormatted = avgResponseTime > 0 ? FormatResponseTime((int)avgResponseTime) : null
            },
            checks = checksList,
            retrievedAt = DateTime.UtcNow
        });
    }
    
    private static string? FormatResponseTime(int responseTimeMs)
    {
        if (responseTimeMs < 1000)
            return $"{responseTimeMs}ms";
        
        var seconds = responseTimeMs / 1000.0;
        return $"{seconds:F2}s";
    }

    /// <summary>
    /// Obtém a última verificação de uptime de um monitor
    /// </summary>
    /// <param name="monitorId">ID do monitor</param>
    /// <returns>Última verificação com informações adicionais</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetLatest(Guid monitorId)
    {
        var check = await _uptimeCheckService.GetLatestByMonitorIdAsync(monitorId);
        if (check == null)
            return NotFound(new { 
                monitorId, 
                message = "Nenhuma verificação de uptime encontrada para este monitor",
                suggestion = "O monitor pode não ter sido verificado ainda ou não existe"
            });

        // Verificar se está desatualizado (última verificação há mais de 1 hora)
        var timeSinceLastCheck = DateTime.UtcNow - check.CheckedAt;
        var isStale = timeSinceLastCheck.TotalHours > 1;

        return Ok(new
        {
            check,
            metadata = new
            {
                isStale,
                stalenessWarning = isStale ? $"Última verificação foi há {Math.Round(timeSinceLastCheck.TotalHours, 1)} horas" : null,
                retrievedAt = DateTime.UtcNow
            }
        });
    }
}
