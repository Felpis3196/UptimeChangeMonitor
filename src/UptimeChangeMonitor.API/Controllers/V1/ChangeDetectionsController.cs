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
    /// <param name="limit">Limite de registros a retornar (opcional)</param>
    /// <returns>Histórico de detecções</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChangeDetectionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChangeDetectionDto>>> GetHistory(
        Guid monitorId,
        [FromQuery] int? limit = null)
    {
        var detections = await _changeDetectionService.GetHistoryByMonitorIdAsync(monitorId, limit);
        return Ok(detections);
    }

    /// <summary>
    /// Obtém a última detecção de mudança de um monitor
    /// </summary>
    /// <param name="monitorId">ID do monitor</param>
    /// <returns>Última detecção</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(ChangeDetectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChangeDetectionDto>> GetLatest(Guid monitorId)
    {
        var detection = await _changeDetectionService.GetLatestByMonitorIdAsync(monitorId);
        if (detection == null)
            return NotFound();

        return Ok(detection);
    }
}
