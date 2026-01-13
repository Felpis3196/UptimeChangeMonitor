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
    /// <param name="limit">Limite de registros a retornar (opcional)</param>
    /// <returns>Histórico de verificações</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UptimeCheckDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UptimeCheckDto>>> GetHistory(
        Guid monitorId,
        [FromQuery] int? limit = null)
    {
        var checks = await _uptimeCheckService.GetHistoryByMonitorIdAsync(monitorId, limit);
        return Ok(checks);
    }

    /// <summary>
    /// Obtém a última verificação de uptime de um monitor
    /// </summary>
    /// <param name="monitorId">ID do monitor</param>
    /// <returns>Última verificação</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(UptimeCheckDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UptimeCheckDto>> GetLatest(Guid monitorId)
    {
        var check = await _uptimeCheckService.GetLatestByMonitorIdAsync(monitorId);
        if (check == null)
            return NotFound();

        return Ok(check);
    }
}
