using Microsoft.Extensions.Logging;
using Museum.Application.Repositories;
using Museum.Contracts.Analytics;
using Museum.Contracts.Excursions;
using Museum.Contracts.Visitors;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Реализация службы аналитики музейной системы.
/// </summary>
/// <param name="exhibitionRepo">Репозиторий выставок.</param>
/// <param name="ticketRepo">Репозиторий билетов.</param>
/// <param name="excursionRepo">Репозиторий экскурсий.</param>
/// <param name="visitorRepo">Репозиторий посетителей.</param>
/// <param name="logger">Журнал аналитических запросов.</param>
public class AnalyticsService(
    IRepository<Exhibition> exhibitionRepo,
    IRepository<Ticket> ticketRepo,
    IRepository<Excursion> excursionRepo,
    IRepository<Visitor> visitorRepo,
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
        .ThenBy(dto => dto.Name)
        .Take(5)
        .ToList();

        return result;
    }

    /// <inheritdoc />
    public IReadOnlyList<ExcursionParticipantsDto> GetExcursionsWithMinimumParticipants()
    {
        logger.LogInformation("Формирование списка экскурсий с минимальным количеством участников.");
        var counts = excursionRepo.GetAll()
            .Select(excursion => new ExcursionParticipantsDto(
                excursion.Id,
                ticketRepo.GetAll().Where(ticket => ticket.ExcursionId == excursion.Id)
                    .Select(ticket => ticket.VisitorId).Distinct().Count()))
            .ToList();
        var minimum = counts.Select(item => item.ParticipantCount).DefaultIfEmpty(0).Min();
        return counts.Where(item => item.ParticipantCount == minimum)
            .OrderBy(item => item.ExcursionId).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<ThemeAttendanceSummaryDto> GetThemeAttendanceSummary(DateTime start, DateTime end)
    {
        logger.LogInformation("Формирование сводной статистики за период с {Start} по {End}.", start, end);

        if (start > end)
            throw new ArgumentException("Дата начала периода должна быть не позже даты окончания.");

        return excursionRepo.GetAll()
            .Where(excursion => excursion.Date.Date >= start.Date && excursion.Date.Date <= end.Date)
            .SelectMany(excursion => excursion.Exhibitions.Select(link => new { excursion, link }))
            .GroupBy(item => item.link.Exhibition.Theme)
            .OrderBy(group => group.Key)
            .Select(group => new ThemeAttendanceSummaryDto(
                group.Key,
                group.Select(item => item.link.ExhibitionId).Distinct().Count(),
                group.Select(item => item.excursion.Id).Distinct().Count(),
                group.SelectMany(item => ticketRepo.GetAll()
                    .Where(ticket => ticket.ExcursionId == item.excursion.Id))
                    .Select(ticket => ticket.VisitorId).Distinct().Count(),
                group.SelectMany(item => ticketRepo.GetAll()
                    .Where(ticket => ticket.ExcursionId == item.excursion.Id))
                    .DistinctBy(ticket => ticket.Id).Sum(ticket => ticket.Price)))
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<ExcursionsDto> GetExcursionsInHall(int hallNumber, DateTime start, DateTime end)
    {
        if (start > end)
            throw new ArgumentException("Дата начала периода должна быть не позже даты окончания.");
        logger.LogInformation("Поиск экскурсий в зале {HallNumber} за период {Start}–{End}.", hallNumber, start, end);
        return excursionRepo.GetAll()
            .Where(excursion => excursion.Date.Date >= start.Date && excursion.Date.Date <= end.Date
                && excursion.Exhibitions.Any(link => link.Exhibition.HallNumber == hallNumber))
            .OrderBy(excursion => excursion.Date)
            .Select(excursion => new ExcursionsDto(excursion.Id, excursion.Date, excursion.Duration,
                excursion.Exhibitions.Select(link => link.ExhibitionId).ToList()))
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<VisitorDto> GetVisitorsForExcursion(int excursionId)
    {
        logger.LogInformation("Поиск посетителей экскурсии {ExcursionId}.", excursionId);
        return ticketRepo.GetAll()
            .Where(ticket => ticket.ExcursionId == excursionId)
            .Select(ticket => ticket.VisitorId)
            .Distinct()
            .Join(visitorRepo.GetAll(), id => id, visitor => visitor.Id,
                (_, visitor) => new VisitorDto(visitor.Id, visitor.FullName, visitor.Phone, visitor.BirthDate))
            .OrderBy(visitor => visitor.FullName)
            .ToList();
    }
}
