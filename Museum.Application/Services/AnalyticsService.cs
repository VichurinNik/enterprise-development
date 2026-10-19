using Microsoft.Extensions.Logging;
using Museum.Application.Repositories;
using Museum.Contracts.Analytics;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Реализация службы аналитики музейной системы.
/// </summary>
public class AnalyticsService(
    IRepository<Exhibition> exhibitionRepo,
    IRepository<Ticket> ticketRepo,
    IRepository<Excursion> excursionRepo,
    ILogger<AnalyticsService> logger) : IAnalyticsService
{
    /// <inheritdoc />
    public IReadOnlyList<TopExhibitionDto> GetTop5Exhibitions()
    {
        logger.LogInformation("Формирование отчета: Топ-5 выставок по посетителям.");

        var tickets = ticketRepo.GetAll();
        var excursions = excursionRepo.GetAll();
        var exhibitions = exhibitionRepo.GetAll();

        var result = exhibitions.Select(ex =>
        {
            var relatedExcursionIds = excursions
                .Where(e => e.Exhibitions.Any(ee => ee.ExhibitionId == ex.Id))
                .Select(e => e.Id)
                .ToHashSet();

            var uniqueVisitorCount = tickets
                .Where(t => relatedExcursionIds.Contains(t.ExcursionId))
                .Select(t => t.VisitorId)
                .Distinct()
                .Count();

            return new TopExhibitionDto(ex.Id, ex.Name, ex.Theme, ex.HallNumber, uniqueVisitorCount);
        })
        .OrderByDescending(dto => dto.VisitorCount)
        .Take(5)
        .ToList();

        return result;
    }

    /// <inheritdoc />
    public IReadOnlyList<ThemeAttendanceSummaryDto> GetThemeAttendanceSummary(DateTime start, DateTime end)
    {
        logger.LogInformation("Формирование сводной статистики за период с {Start} по {End}.", start, end);

        // Временный пустой ответ до интеграции полноценных запросов LINQ из 1-й лабы
        return [];
    }
}