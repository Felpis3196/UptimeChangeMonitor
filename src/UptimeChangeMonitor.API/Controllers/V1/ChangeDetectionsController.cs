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
    /// <returns>Histórico de detecções com estatísticas detalhadas e dados para visualização</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetHistory(
        Guid monitorId,
        [FromQuery] int? limit = null)
    {
        var detections = await _changeDetectionService.GetHistoryByMonitorIdAsync(monitorId, limit);
        var detectionsList = detections.ToList();
        
        var now = DateTime.UtcNow;
        var last24h = now.AddHours(-24);
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);
        
        // Filtrar por períodos
        var detections24h = detectionsList.Where(d => d.DetectedAt >= last24h).ToList();
        var detections7Days = detectionsList.Where(d => d.DetectedAt >= last7Days).ToList();
        var detections30Days = detectionsList.Where(d => d.DetectedAt >= last30Days).ToList();
        
        // Calcular estatísticas gerais
        var total = detectionsList.Count;
        var contentChanged = detectionsList.Count(d => d.ChangeType == Domain.Enums.ChangeType.ContentChanged);
        var structureChanged = detectionsList.Count(d => d.ChangeType == Domain.Enums.ChangeType.StructureChanged);
        var statusChanged = detectionsList.Count(d => d.ChangeType == Domain.Enums.ChangeType.StatusChanged);
        var significantChanges = detectionsList.Count(d => d.HasSignificantChange);
        
        var lastChange = detectionsList.OrderByDescending(d => d.DetectedAt).FirstOrDefault();
        var firstChange = detectionsList.OrderBy(d => d.DetectedAt).FirstOrDefault();
        
        var timeSinceLastChange = lastChange != null 
            ? now - lastChange.DetectedAt 
            : (TimeSpan?)null;
        
        var timeSpan = firstChange != null && lastChange != null
            ? lastChange.DetectedAt - firstChange.DetectedAt
            : (TimeSpan?)null;
        
        var changeFrequency = total > 0 && timeSpan.HasValue && timeSpan.Value.TotalDays > 0
            ? total / timeSpan.Value.TotalDays
            : (double?)null;
        
        // Calcular estatísticas por período
        var stats24h = CalculatePeriodStats(detections24h, "24 horas");
        var stats7Days = CalculatePeriodStats(detections7Days, "7 dias");
        var stats30Days = CalculatePeriodStats(detections30Days, "30 dias");
        
        // Preparar dados para gráficos (série temporal)
        var timeSeriesData = PrepareTimeSeriesData(detectionsList);
        
        // Distribuição por tipo de mudança
        var changeTypeDistribution = new[]
        {
            new { type = "ContentChanged", label = "Conteúdo Alterado", count = contentChanged, percentage = total > 0 ? Math.Round((contentChanged * 100.0 / total), 2) : 0 },
            new { type = "StructureChanged", label = "Estrutura Alterada", count = structureChanged, percentage = total > 0 ? Math.Round((structureChanged * 100.0 / total), 2) : 0 },
            new { type = "StatusChanged", label = "Status Alterado", count = statusChanged, percentage = total > 0 ? Math.Round((statusChanged * 100.0 / total), 2) : 0 }
        };
        
        // Mudanças por dia da semana
        var changesByDayOfWeek = detectionsList
            .GroupBy(d => d.DetectedAt.DayOfWeek)
            .Select(g => new
            {
                dayOfWeek = g.Key.ToString(),
                dayOfWeekPt = GetDayOfWeekPt(g.Key),
                count = g.Count(),
                percentage = Math.Round((g.Count() * 100.0 / total), 2),
                dayOfWeekOrder = (int)g.Key
            })
            .OrderBy(x => x.dayOfWeekOrder)
            .ToList();
        
        // Mudanças por hora do dia
        var changesByHour = detectionsList
            .GroupBy(d => d.DetectedAt.Hour)
            .Select(g => new
            {
                hour = g.Key,
                hourFormatted = $"{g.Key:00}:00",
                count = g.Count(),
                percentage = Math.Round((g.Count() * 100.0 / total), 2)
            })
            .OrderBy(x => x.hour)
            .ToList();
        
        // Tendências
        var trends = CalculateChangeTrends(detectionsList);
        
        // Intervalo entre mudanças
        var intervals = CalculateChangeIntervals(detectionsList);

        return Ok(new
        {
            monitorId,
            metadata = new
            {
                totalRecords = total,
                dateRange = new
                {
                    from = firstChange?.DetectedAt,
                    to = lastChange?.DetectedAt,
                    span = timeSpan.HasValue ? timeSpan.Value.TotalDays : (double?)null
                },
                retrievedAt = now
            },
            summary = new
            {
                overall = new
                {
                    total,
                    contentChanged,
                    structureChanged,
                    statusChanged,
                    significantChanges,
                    changeTypeDistribution = changeTypeDistribution,
                    significantChangesPercentage = total > 0 ? Math.Round((significantChanges * 100.0 / total), 2) : 0
                },
                timing = new
                {
                    firstChangeAt = firstChange?.DetectedAt,
                    firstChangeAtFormatted = firstChange?.DetectedAtFormatted,
                    lastChangeAt = lastChange?.DetectedAt,
                    lastChangeAtFormatted = lastChange?.DetectedAtFormatted,
                    timeSinceLastChange = timeSinceLastChange.HasValue 
                        ? GetTimeAgoFormatted(timeSinceLastChange.Value) 
                        : null,
                    timeSinceLastChangeHours = timeSinceLastChange.HasValue ? Math.Round(timeSinceLastChange.Value.TotalHours, 2) : (double?)null,
                    changeFrequency = changeFrequency.HasValue ? Math.Round(changeFrequency.Value, 2) : (double?)null,
                    changeFrequencyFormatted = changeFrequency.HasValue 
                        ? $"{Math.Round(changeFrequency.Value, 2)} mudanças por dia" 
                        : null
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
                changeTypeDistribution = changeTypeDistribution,
                changesByDayOfWeek = changesByDayOfWeek,
                changesByHour = changesByHour
            },
            intervals = intervals,
            trends = trends,
            detections = detectionsList.OrderByDescending(d => d.DetectedAt).ToList()
        });
    }
    
    private static object CalculatePeriodStats(List<ChangeDetectionDto> detections, string periodName)
    {
        if (!detections.Any())
            return new { period = periodName, total = 0 };
        
        return new
        {
            period = periodName,
            total = detections.Count,
            contentChanged = detections.Count(d => d.ChangeType == Domain.Enums.ChangeType.ContentChanged),
            structureChanged = detections.Count(d => d.ChangeType == Domain.Enums.ChangeType.StructureChanged),
            statusChanged = detections.Count(d => d.ChangeType == Domain.Enums.ChangeType.StatusChanged),
            significantChanges = detections.Count(d => d.HasSignificantChange)
        };
    }
    
    private static List<object> PrepareTimeSeriesData(List<ChangeDetectionDto> detections)
    {
        return detections
            .OrderBy(d => d.DetectedAt)
            .GroupBy(d => new DateTime(d.DetectedAt.Year, d.DetectedAt.Month, d.DetectedAt.Day, d.DetectedAt.Hour, 0, 0))
            .Select(g => new
            {
                timestamp = g.Key,
                timestampFormatted = g.Key.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                count = g.Count(),
                contentChanged = g.Count(d => d.ChangeType == Domain.Enums.ChangeType.ContentChanged),
                structureChanged = g.Count(d => d.ChangeType == Domain.Enums.ChangeType.StructureChanged),
                statusChanged = g.Count(d => d.ChangeType == Domain.Enums.ChangeType.StatusChanged),
                significantChanges = g.Count(d => d.HasSignificantChange)
            })
            .Cast<object>()
            .ToList();
    }
    
    private static string GetDayOfWeekPt(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => "Domingo",
            DayOfWeek.Monday => "Segunda-feira",
            DayOfWeek.Tuesday => "Terça-feira",
            DayOfWeek.Wednesday => "Quarta-feira",
            DayOfWeek.Thursday => "Quinta-feira",
            DayOfWeek.Friday => "Sexta-feira",
            DayOfWeek.Saturday => "Sábado",
            _ => dayOfWeek.ToString()
        };
    }
    
    private static object CalculateChangeTrends(List<ChangeDetectionDto> detections)
    {
        if (detections.Count < 2)
            return new { available = false, message = "Dados insuficientes para calcular tendências" };
        
        var sorted = detections.OrderBy(d => d.DetectedAt).ToList();
        var firstHalf = sorted.Take(sorted.Count / 2).ToList();
        var secondHalf = sorted.Skip(sorted.Count / 2).ToList();
        
        var firstHalfCount = firstHalf.Count;
        var secondHalfCount = secondHalf.Count;
        
        var timeSpanFirst = firstHalfCount > 1 
            ? (firstHalf.Last().DetectedAt - firstHalf.First().DetectedAt).TotalDays 
            : 0;
        var timeSpanSecond = secondHalfCount > 1 
            ? (secondHalf.Last().DetectedAt - secondHalf.First().DetectedAt).TotalDays 
            : 0;
        
        var firstHalfRate = timeSpanFirst > 0 ? firstHalfCount / timeSpanFirst : 0;
        var secondHalfRate = timeSpanSecond > 0 ? secondHalfCount / timeSpanSecond : 0;
        
        var rateChange = secondHalfRate - firstHalfRate;
        
        return new
        {
            available = true,
            changeRate = new
            {
                firstHalf = Math.Round(firstHalfRate, 2),
                secondHalf = Math.Round(secondHalfRate, 2),
                change = Math.Round(rateChange, 2),
                direction = rateChange > 0 ? "aumentando" : rateChange < 0 ? "diminuindo" : "estável",
                isIncreasing = rateChange > 0
            }
        };
    }
    
    private static object CalculateChangeIntervals(List<ChangeDetectionDto> detections)
    {
        if (detections.Count < 2)
            return new { available = false, message = "Dados insuficientes para calcular intervalos" };
        
        var sorted = detections.OrderBy(d => d.DetectedAt).ToList();
        var intervals = new List<double>();
        
        for (int i = 1; i < sorted.Count; i++)
        {
            var interval = (sorted[i].DetectedAt - sorted[i - 1].DetectedAt).TotalHours;
            intervals.Add(interval);
        }
        
        if (!intervals.Any())
            return new { available = false };
        
        return new
        {
            available = true,
            average = Math.Round(intervals.Average(), 2),
            averageFormatted = $"{Math.Round(intervals.Average(), 2)} horas",
            min = Math.Round(intervals.Min(), 2),
            minFormatted = $"{Math.Round(intervals.Min(), 2)} horas",
            max = Math.Round(intervals.Max(), 2),
            maxFormatted = $"{Math.Round(intervals.Max(), 2)} horas",
            median = Math.Round(CalculateMedian(intervals), 2),
            medianFormatted = $"{Math.Round(CalculateMedian(intervals), 2)} horas"
        };
    }
    
    private static double CalculateMedian(List<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        var count = sorted.Count;
        if (count == 0) return 0;
        if (count % 2 == 0)
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2;
        return sorted[count / 2];
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
