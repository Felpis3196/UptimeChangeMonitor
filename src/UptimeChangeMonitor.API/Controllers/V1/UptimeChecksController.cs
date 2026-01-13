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
    /// <returns>Histórico de verificações com estatísticas detalhadas e dados para visualização</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetHistory(
        Guid monitorId,
        [FromQuery] int? limit = null)
    {
        var checks = await _uptimeCheckService.GetHistoryByMonitorIdAsync(monitorId, limit);
        var checksList = checks.ToList();
        
        var now = DateTime.UtcNow;
        var last24h = now.AddHours(-24);
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);
        
        // Filtrar por períodos
        var checks24h = checksList.Where(c => c.CheckedAt >= last24h).ToList();
        var checks7Days = checksList.Where(c => c.CheckedAt >= last7Days).ToList();
        var checks30Days = checksList.Where(c => c.CheckedAt >= last30Days).ToList();
        
        // Calcular estatísticas gerais
        var total = checksList.Count;
        var online = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Online);
        var offline = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Offline);
        var timeout = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Timeout);
        var error = checksList.Count(c => c.Status == Domain.Enums.UptimeStatus.Error);
        
        var responseTimes = checksList.Where(c => c.ResponseTimeMs.HasValue).Select(c => c.ResponseTimeMs!.Value).ToList();
        var avgResponseTime = responseTimes.Any() ? responseTimes.Average() : 0;
        var minResponseTime = responseTimes.Any() ? responseTimes.Min() : (int?)null;
        var maxResponseTime = responseTimes.Any() ? responseTimes.Max() : (int?)null;
        var medianResponseTime = responseTimes.Any() ? CalculateMedian(responseTimes) : (int?)null;
        
        var uptimePercentage = total > 0 ? (online * 100.0 / total) : 0;
        
        // Calcular estatísticas por período
        var stats24h = CalculatePeriodStats(checks24h, "24 horas");
        var stats7Days = CalculatePeriodStats(checks7Days, "7 dias");
        var stats30Days = CalculatePeriodStats(checks30Days, "30 dias");
        
        // Preparar dados para gráficos (série temporal)
        var timeSeriesData = PrepareTimeSeriesData(checksList);
        
        // Status codes distribution
        var statusCodeDistribution = checksList
            .Where(c => c.StatusCode.HasValue)
            .GroupBy(c => c.StatusCode!.Value)
            .Select(g => new { statusCode = g.Key, count = g.Count(), percentage = Math.Round((g.Count() * 100.0 / total), 2) })
            .OrderByDescending(x => x.count)
            .ToList();
        
        // Response time distribution (buckets)
        var responseTimeBuckets = CalculateResponseTimeBuckets(responseTimes);
        
        // Tendências
        var trends = CalculateTrends(checksList);
        
        // Primeira e última verificação
        var firstCheck = checksList.OrderBy(c => c.CheckedAt).FirstOrDefault();
        var lastCheck = checksList.OrderByDescending(c => c.CheckedAt).FirstOrDefault();

        return Ok(new
        {
            monitorId,
            metadata = new
            {
                totalRecords = total,
                dateRange = new
                {
                    from = firstCheck?.CheckedAt,
                    to = lastCheck?.CheckedAt,
                    span = firstCheck != null && lastCheck != null 
                        ? (lastCheck.CheckedAt - firstCheck.CheckedAt).TotalDays 
                        : (double?)null
                },
                retrievedAt = now
            },
            summary = new
            {
                overall = new
                {
                    total,
                    online,
                    offline,
                    timeout,
                    error,
                    uptimePercentage = Math.Round(uptimePercentage, 2),
                    downtimePercentage = Math.Round(100 - uptimePercentage, 2),
                    statusDistribution = new
                    {
                        online = new { count = online, percentage = total > 0 ? Math.Round((online * 100.0 / total), 2) : 0 },
                        offline = new { count = offline, percentage = total > 0 ? Math.Round((offline * 100.0 / total), 2) : 0 },
                        timeout = new { count = timeout, percentage = total > 0 ? Math.Round((timeout * 100.0 / total), 2) : 0 },
                        error = new { count = error, percentage = total > 0 ? Math.Round((error * 100.0 / total), 2) : 0 }
                    }
                },
                responseTime = new
                {
                    average = avgResponseTime > 0 ? Math.Round(avgResponseTime, 2) : (double?)null,
                    averageFormatted = avgResponseTime > 0 ? FormatResponseTime((int)avgResponseTime) : null,
                    min = minResponseTime,
                    minFormatted = minResponseTime.HasValue ? FormatResponseTime(minResponseTime.Value) : null,
                    max = maxResponseTime,
                    maxFormatted = maxResponseTime.HasValue ? FormatResponseTime(maxResponseTime.Value) : null,
                    median = medianResponseTime,
                    medianFormatted = medianResponseTime.HasValue ? FormatResponseTime(medianResponseTime.Value) : null,
                    samples = responseTimes.Count
                },
                byPeriod = new
                {
                    last24Hours = stats24h,
                    last7Days = stats7Days,
                    last30Days = stats30Days
                }
            },
            charts = new
            {
                timeSeries = timeSeriesData,
                responseTimeDistribution = responseTimeBuckets,
                statusCodeDistribution = statusCodeDistribution
            },
            trends = trends,
            checks = checksList.OrderByDescending(c => c.CheckedAt).ToList()
        });
    }
    
    private static object CalculatePeriodStats(List<UptimeCheckDto> checks, string periodName)
    {
        if (!checks.Any())
            return new { period = periodName, total = 0, uptimePercentage = 0.0, averageResponseTime = (double?)null };
        
        var total = checks.Count;
        var online = checks.Count(c => c.Status == Domain.Enums.UptimeStatus.Online);
        var uptimePercentage = (online * 100.0 / total);
        var responseTimes = checks.Where(c => c.ResponseTimeMs.HasValue).Select(c => c.ResponseTimeMs!.Value).ToList();
        var avgResponseTime = responseTimes.Any() ? responseTimes.Average() : (double?)null;
        
        return new
        {
            period = periodName,
            total,
            online,
            offline = checks.Count(c => c.Status == Domain.Enums.UptimeStatus.Offline),
            timeout = checks.Count(c => c.Status == Domain.Enums.UptimeStatus.Timeout),
            error = checks.Count(c => c.Status == Domain.Enums.UptimeStatus.Error),
            uptimePercentage = Math.Round(uptimePercentage, 2),
            averageResponseTime = avgResponseTime.HasValue ? Math.Round(avgResponseTime.Value, 2) : (double?)null,
            averageResponseTimeFormatted = avgResponseTime.HasValue ? FormatResponseTime((int)avgResponseTime.Value) : null
        };
    }
    
    private static List<object> PrepareTimeSeriesData(List<UptimeCheckDto> checks)
    {
        // Agrupar por hora para criar série temporal
        return checks
            .OrderBy(c => c.CheckedAt)
            .GroupBy(c => new DateTime(c.CheckedAt.Year, c.CheckedAt.Month, c.CheckedAt.Day, c.CheckedAt.Hour, 0, 0))
            .Select(g => new
            {
                timestamp = g.Key,
                timestampFormatted = g.Key.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                count = g.Count(),
                online = g.Count(c => c.Status == Domain.Enums.UptimeStatus.Online),
                offline = g.Count(c => c.Status == Domain.Enums.UptimeStatus.Offline),
                timeout = g.Count(c => c.Status == Domain.Enums.UptimeStatus.Timeout),
                error = g.Count(c => c.Status == Domain.Enums.UptimeStatus.Error),
                uptimePercentage = g.Count() > 0 ? Math.Round((g.Count(c => c.Status == Domain.Enums.UptimeStatus.Online) * 100.0 / g.Count()), 2) : 0,
                averageResponseTime = g.Where(c => c.ResponseTimeMs.HasValue).Any()
                    ? Math.Round(g.Where(c => c.ResponseTimeMs.HasValue).Average(c => c.ResponseTimeMs!.Value), 2)
                    : (double?)null
            })
            .Cast<object>()
            .ToList();
    }
    
    private static List<object> CalculateResponseTimeBuckets(List<int> responseTimes)
    {
        if (!responseTimes.Any())
            return new List<object>();
        
        var buckets = new[]
        {
            new { label = "0-100ms", min = 0, max = 100 },
            new { label = "100-250ms", min = 100, max = 250 },
            new { label = "250-500ms", min = 250, max = 500 },
            new { label = "500ms-1s", min = 500, max = 1000 },
            new { label = "1s-2s", min = 1000, max = 2000 },
            new { label = "2s+", min = 2000, max = int.MaxValue }
        };
        
        var total = responseTimes.Count;
        
        return buckets.Select(bucket =>
        {
            var count = responseTimes.Count(rt => rt >= bucket.min && rt < bucket.max);
            return new
            {
                bucket = bucket.label,
                count,
                percentage = Math.Round((count * 100.0 / total), 2)
            };
        }).Cast<object>().ToList();
    }
    
    private static object CalculateTrends(List<UptimeCheckDto> checks)
    {
        if (checks.Count < 2)
            return new { available = false, message = "Dados insuficientes para calcular tendências" };
        
        var sorted = checks.OrderBy(c => c.CheckedAt).ToList();
        var firstHalf = sorted.Take(sorted.Count / 2).ToList();
        var secondHalf = sorted.Skip(sorted.Count / 2).ToList();
        
        var firstHalfUptime = firstHalf.Count > 0 
            ? (firstHalf.Count(c => c.Status == Domain.Enums.UptimeStatus.Online) * 100.0 / firstHalf.Count) 
            : 0;
        var secondHalfUptime = secondHalf.Count > 0 
            ? (secondHalf.Count(c => c.Status == Domain.Enums.UptimeStatus.Online) * 100.0 / secondHalf.Count) 
            : 0;
        
        var uptimeTrend = secondHalfUptime - firstHalfUptime;
        
        var firstHalfAvgResponse = firstHalf.Where(c => c.ResponseTimeMs.HasValue).Any()
            ? firstHalf.Where(c => c.ResponseTimeMs.HasValue).Average(c => c.ResponseTimeMs!.Value)
            : (double?)null;
        var secondHalfAvgResponse = secondHalf.Where(c => c.ResponseTimeMs.HasValue).Any()
            ? secondHalf.Where(c => c.ResponseTimeMs.HasValue).Average(c => c.ResponseTimeMs!.Value)
            : (double?)null;
        
        var responseTimeTrend = firstHalfAvgResponse.HasValue && secondHalfAvgResponse.HasValue
            ? secondHalfAvgResponse.Value - firstHalfAvgResponse.Value
            : (double?)null;
        
        return new
        {
            available = true,
            uptime = new
            {
                firstHalf = Math.Round(firstHalfUptime, 2),
                secondHalf = Math.Round(secondHalfUptime, 2),
                change = Math.Round(uptimeTrend, 2),
                direction = uptimeTrend > 0 ? "melhorando" : uptimeTrend < 0 ? "piorando" : "estável",
                isImproving = uptimeTrend > 0
            },
            responseTime = new
            {
                firstHalf = firstHalfAvgResponse.HasValue ? Math.Round(firstHalfAvgResponse.Value, 2) : (double?)null,
                secondHalf = secondHalfAvgResponse.HasValue ? Math.Round(secondHalfAvgResponse.Value, 2) : (double?)null,
                change = responseTimeTrend.HasValue ? Math.Round(responseTimeTrend.Value, 2) : (double?)null,
                direction = responseTimeTrend.HasValue 
                    ? (responseTimeTrend.Value < 0 ? "melhorando" : responseTimeTrend.Value > 0 ? "piorando" : "estável")
                    : null,
                isImproving = responseTimeTrend.HasValue && responseTimeTrend.Value < 0
            }
        };
    }
    
    private static int CalculateMedian(List<int> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        var count = sorted.Count;
        if (count == 0) return 0;
        if (count % 2 == 0)
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2;
        return sorted[count / 2];
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
