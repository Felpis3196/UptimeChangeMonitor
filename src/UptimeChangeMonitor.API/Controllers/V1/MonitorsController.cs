using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using UptimeChangeMonitor.Application.DTOs;
using UptimeChangeMonitor.Application.Interfaces;

namespace UptimeChangeMonitor.API.Controllers.V1;

/// <summary>
/// Controller para gerenciar monitores de sites (v1)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class MonitorsController : ControllerBase
{
    private readonly IMonitorService _monitorService;
    private readonly IUptimeCheckService _uptimeCheckService;
    private readonly IChangeDetectionService _changeDetectionService;

    public MonitorsController(
        IMonitorService monitorService,
        IUptimeCheckService uptimeCheckService,
        IChangeDetectionService changeDetectionService)
    {
        _monitorService = monitorService;
        _uptimeCheckService = uptimeCheckService;
        _changeDetectionService = changeDetectionService;
    }

    /// <summary>
    /// Lista todos os monitores cadastrados
    /// </summary>
    /// <returns>Lista de monitores</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MonitorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MonitorDto>>> GetAll()
    {
        var monitors = await _monitorService.GetAllAsync();
        return Ok(monitors);
    }

    /// <summary>
    /// Busca um monitor pelo ID
    /// </summary>
    /// <param name="id">ID do monitor</param>
    /// <returns>Dados do monitor</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MonitorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MonitorDto>> GetById(Guid id)
    {
        var monitor = await _monitorService.GetByIdAsync(id);
        if (monitor == null)
            return NotFound();

        return Ok(monitor);
    }

    /// <summary>
    /// Cria um novo monitor
    /// </summary>
    /// <param name="createDto">Dados do monitor a ser criado</param>
    /// <returns>Monitor criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MonitorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MonitorDto>> Create([FromBody] CreateMonitorDto createDto)
    {
        var monitor = await _monitorService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = monitor.Id, version = "1.0" }, monitor);
    }

    /// <summary>
    /// Atualiza um monitor existente
    /// </summary>
    /// <param name="id">ID do monitor</param>
    /// <param name="updateDto">Dados atualizados do monitor</param>
    /// <returns>Monitor atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MonitorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MonitorDto>> Update(Guid id, [FromBody] UpdateMonitorDto updateDto)
    {
        var monitor = await _monitorService.UpdateAsync(id, updateDto);
        if (monitor == null)
            return NotFound();

        return Ok(monitor);
    }

    /// <summary>
    /// Deleta um monitor
    /// </summary>
    /// <param name="id">ID do monitor</param>
    /// <returns>No content se deletado com sucesso</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _monitorService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Obtém o status atual de um monitor com informações detalhadas
    /// </summary>
    /// <param name="id">ID do monitor</param>
    /// <returns>Status do monitor com estatísticas e informações agregadas</returns>
    [HttpGet("{id}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetStatus(Guid id)
    {
        var status = await _monitorService.GetStatusAsync(id);
        if (status == null)
            return NotFound(new
            {
                monitorId = id,
                message = "Monitor não encontrado",
                suggestion = "Verifique se o ID do monitor está correto"
            });

        // Buscar estatísticas rápidas das últimas verificações
        var recentChecks = await _uptimeCheckService.GetHistoryByMonitorIdAsync(id, 10);
        var recentChanges = await _changeDetectionService.GetHistoryByMonitorIdAsync(id, 5);
        
        var healthScore = CalculateHealthScore(status, recentChecks);

        return Ok(new
        {
            status,
            quickStats = new
            {
                recentChecks = recentChecks.Select(c => new
                {
                    c.Status,
                    c.StatusDescription,
                    c.IsOnline,
                    c.ResponseTimeMs,
                    c.ResponseTimeFormatted,
                    CheckedAt = c.CheckedAtFormatted ?? c.CheckedAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                    c.CheckedAtFormatted,
                    c.TimeAgo
                }),
                recentChanges = recentChanges.Select(c => new
                {
                    c.ChangeType,
                    c.ChangeTypeDescription,
                    DetectedAt = c.DetectedAtFormatted ?? c.DetectedAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                    c.DetectedAtFormatted,
                    c.TimeAgo,
                    c.HasSignificantChange
                })
            },
            health = healthScore,
            metadata = new
            {
                RetrievedAt = DateTime.UtcNow,
                MonitorId = id
            }
        });
    }
    
    private static object CalculateHealthScore(MonitorStatusDto status, IEnumerable<UptimeCheckDto> recentChecks)
    {
        var checks = recentChecks.ToList();
        if (!checks.Any())
            return new 
            { 
                score = 0, 
                level = "unknown", 
                description = "Sem dados suficientes",
                uptimePercentage = 0.0,
                averageResponseTime = (double?)null
            };
        
        var onlineCount = checks.Count(c => c.IsOnline);
        var uptimePercentage = (onlineCount * 100.0 / checks.Count);
        
        var avgResponseTime = checks
            .Where(c => c.ResponseTimeMs.HasValue)
            .Select(c => c.ResponseTimeMs!.Value)
            .DefaultIfEmpty(0)
            .Average();
        
        // Calcular score (0-100)
        var uptimeScore = uptimePercentage;
        var responseTimeScore = avgResponseTime > 0 
            ? Math.Max(0, 100 - (avgResponseTime / 10)) // Penaliza tempos > 1s
            : 50;
        
        var totalScore = (uptimeScore * 0.7 + responseTimeScore * 0.3);
        
        var level = totalScore >= 90 ? "excellent" 
            : totalScore >= 75 ? "good"
            : totalScore >= 50 ? "fair"
            : totalScore >= 25 ? "poor"
            : "critical";
        
        var description = level switch
        {
            "excellent" => "Monitor está funcionando perfeitamente",
            "good" => "Monitor está funcionando bem",
            "fair" => "Monitor apresenta alguns problemas",
            "poor" => "Monitor apresenta problemas significativos",
            "critical" => "Monitor está com problemas críticos",
            _ => "Status desconhecido"
        };
        
        return new
        {
            score = Math.Round(totalScore, 2),
            level,
            description,
            uptimePercentage = Math.Round(uptimePercentage, 2),
            averageResponseTime = avgResponseTime > 0 ? Math.Round(avgResponseTime, 2) : (double?)null
        };
    }

    /// <summary>
    /// Obtém o status de todos os monitores com informações agregadas
    /// </summary>
    /// <returns>Lista com status de todos os monitores e estatísticas gerais</returns>
    [HttpGet("status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetAllStatus()
    {
        var statusList = await _monitorService.GetAllStatusAsync();
        var statusListArray = statusList.ToList();
        
        // Calcular estatísticas gerais
        var totalMonitors = statusListArray.Count;
        var onlineMonitors = statusListArray.Count(s => s.IsOnline);
        var offlineMonitors = statusListArray.Count(s => !s.IsOnline);
        var monitorsWithRecentChanges = statusListArray.Count(s => s.HasRecentChanges);
        
        var avgResponseTime = statusListArray
            .Where(s => s.LastResponseTimeMs.HasValue)
            .Select(s => s.LastResponseTimeMs!.Value)
            .DefaultIfEmpty(0)
            .Average();
        
        var overallUptime = totalMonitors > 0 ? (onlineMonitors * 100.0 / totalMonitors) : 0;
        
        // Agrupar por status
        var byStatus = statusListArray
            .Where(s => s.CurrentStatus.HasValue)
            .GroupBy(s => s.CurrentStatus!.Value)
            .Select(g => new
            {
                status = g.Key.ToString(),
                count = g.Count(),
                percentage = Math.Round((g.Count() * 100.0 / totalMonitors), 2)
            })
            .ToList();

        return Ok(new
        {
            summary = new
            {
                totalMonitors,
                onlineMonitors,
                offlineMonitors,
                monitorsWithRecentChanges,
                overallUptimePercentage = Math.Round(overallUptime, 2),
                averageResponseTimeMs = avgResponseTime > 0 ? Math.Round(avgResponseTime, 2) : (double?)null,
                averageResponseTimeFormatted = avgResponseTime > 0 ? FormatResponseTime((int)avgResponseTime) : null,
                statusDistribution = byStatus
            },
            monitors = statusListArray,
            metadata = new
            {
                RetrievedAt = DateTime.UtcNow
            }
        });
    }
    
    private static string? FormatResponseTime(int responseTimeMs)
    {
        if (responseTimeMs < 1000)
            return $"{responseTimeMs}ms";
        
        var seconds = responseTimeMs / 1000.0;
        return $"{seconds:F2}s";
    }
}
