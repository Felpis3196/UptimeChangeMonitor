using UptimeChangeMonitor.Application.DTOs;

namespace UptimeChangeMonitor.Application.Interfaces;

public interface IMonitorService
{
    Task<MonitorDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MonitorDto>> GetAllAsync();
    Task<MonitorDto> CreateAsync(CreateMonitorDto createDto);
    Task<MonitorDto?> UpdateAsync(Guid id, UpdateMonitorDto updateDto);
    Task<bool> DeleteAsync(Guid id);
    Task<MonitorStatusDto?> GetStatusAsync(Guid id);
    Task<IEnumerable<MonitorStatusDto>> GetAllStatusAsync();
}
