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

    public MonitorsController(IMonitorService monitorService)
    {
        _monitorService = monitorService;
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
    /// Obtém o status atual de um monitor
    /// </summary>
    /// <param name="id">ID do monitor</param>
    /// <returns>Status do monitor (última verificação, se está online, etc)</returns>
    [HttpGet("{id}/status")]
    [ProducesResponseType(typeof(MonitorStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MonitorStatusDto>> GetStatus(Guid id)
    {
        var status = await _monitorService.GetStatusAsync(id);
        if (status == null)
            return NotFound();

        return Ok(status);
    }

    /// <summary>
    /// Obtém o status de todos os monitores
    /// </summary>
    /// <returns>Lista com status de todos os monitores</returns>
    [HttpGet("status")]
    [ProducesResponseType(typeof(IEnumerable<MonitorStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MonitorStatusDto>>> GetAllStatus()
    {
        var statusList = await _monitorService.GetAllStatusAsync();
        return Ok(statusList);
    }
}
