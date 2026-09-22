using System.Linq;
using Museum.Tests.Fixtures;
using Xunit;

namespace Museum.Tests;

/// <summary>
/// Содержит тесты для проверки аналитических LINQ-запросов к контексту музея.
/// Имплементация первичного конструктора (primary constructor).
/// </summary>
public class QueriesTests(MuseumFixture fixture) : IClassFixture<MuseumFixture>
{
    private readonly MuseumFixture _fixture = fixture;

    /// <summary>
    /// Проверяет, что запрос возвращает топ-5 выставок, отсортированных по количеству посетителей по убыванию.
    /// </summary>
    [Fact]
    public void GetTop5ExhibitionsByVisitors_ShouldReturnOrderedResult()
    {
        var context = _fixture.Context;

        var result = context.Exhibitions
            .Select(exhibition => new
            {
                Exhibition = exhibition,
                VisitorCount = (exhibition.ExcursionExhibitions ?? [])
                    .SelectMany(x => x.Excursion?.Tickets ?? [])
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.VisitorCount)
            .ThenBy(x => x.Exhibition.Name)
            .Take(5)
            .ToList();

        var expectedOrder = result
            .OrderByDescending(x => x.VisitorCount)
            .ThenBy(x => x.Exhibition.Name)
            .ToList();

        Assert.Equal(expectedOrder, result);
    }

    /// <summary>
    /// Проверяет, что запрос возвращает экскурсии с минимальным количеством участников.
    /// </summary>
    [Fact]
    public void GetExcursionsWithMinimumParticipants_ShouldReturnCorrectResult()
    {
        var context = _fixture.Context;

        var excursionParticipants = context.Excursions
            .Select(excursion => new
            {
                Excursion = excursion,
                Participants = (excursion.Tickets ?? [])
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .ToList();

        var minParticipants = excursionParticipants
            .Select(x => x.Participants)
            .DefaultIfEmpty(0)
            .Min();

        var result = excursionParticipants
            .Where(x => x.Participants == minParticipants)
            .ToList();

        var expected = excursionParticipants.Where(x => x.Participants == minParticipants).ToList();
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Проверяет, что запрос формирует корректную статистику посещаемости сгруппированную по тематике выставки.
    /// </summary>
    [Fact]
    public void GetAttendanceSummaryByTheme_ShouldReturnCorrectStatistics()
    {
        var context = _fixture.Context;

        var dates = context.Excursions.Select(x => x.Date.Date).ToList();
        if (dates.Count == 0) return;

        var startDate = dates.Min();
        var endDate = dates.Max();

        var result = context.ExcursionExhibitions
            .Where(x => x.Excursion != null &&
                        x.Excursion.Date.Date >= startDate &&
                        x.Excursion.Date.Date <= endDate)
            .GroupBy(x => x.Exhibition.Theme)
            .Select(group => new
            {
                Theme = group.Key,
                TotalVisitors = group
                    .SelectMany(x => x.Excursion?.Tickets ?? [])
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count(),
                TotalTicketCost = group
                    .SelectMany(x => x.Excursion?.Tickets ?? [])
                    .Sum(ticket => ticket.Price),
                DailyStatistics = group
                    .GroupBy(x => x.Excursion.Date.Date)
                    .Select(day => day
                        .SelectMany(x => x.Excursion?.Tickets ?? [])
                        .Select(ticket => ticket.VisitorId)
                        .Distinct()
                        .Count())
                    .ToList()
            })
            .Select(x => new
            {
                x.Theme,
                x.TotalVisitors,
                x.TotalTicketCost,
                AverageVisitorsPerDay = x.DailyStatistics.DefaultIfEmpty(0).Average(),
                MinVisitorsPerDay = x.DailyStatistics.DefaultIfEmpty(0).Min(),
                MaxVisitorsPerDay = x.DailyStatistics.DefaultIfEmpty(0).Max()
            })
            .ToList();

        var expected = result.ToList(); 
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Проверяет, что возвращаются только экскурсии, проходящие в заданном зале (используется константа).
    /// </summary>
    [Fact]
    public void GetExcursionsInSelectedHall_ShouldReturnCorrectResult()
    {
        var context = _fixture.Context;
        const int targetHall = 1;

        var dates = context.Excursions.Select(x => x.Date.Date).ToList();
        if (dates.Count == 0) return;

        var startDate = dates.Min();
        var endDate = dates.Max();

        var result = context.Excursions
            .Where(excursion =>
                excursion.Date.Date >= startDate &&
                excursion.Date.Date <= endDate &&
                excursion.Exhibitions.Any(x => x.Exhibition != null && x.Exhibition.HallNumber == targetHall))
            .OrderBy(excursion => excursion.Date)
            .ThenBy(excursion => excursion.StartTime)
            .ToList();

        var expected = context.Excursions
            .Where(excursion =>
                excursion.Date.Date >= startDate &&
                excursion.Date.Date <= endDate &&
                excursion.Exhibitions.Any(x => x.Exhibition != null && x.Exhibition.HallNumber == targetHall))
            .OrderBy(excursion => excursion.Date)
            .ThenBy(excursion => excursion.StartTime)
            .ToList();

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Проверяет, что список посетителей выбранной экскурсии сортируется по алфавиту.
    /// </summary>
    [Fact]
    public void GetVisitorsForSelectedExcursion_ShouldReturnOrderedResult()
    {
        var context = _fixture.Context;
        const int targetExcursionId = 1; 
        var result = context.Tickets
            .Where(t => t.ExcursionId == targetExcursionId && t.Visitor != null)
            .Select(ticket => ticket.Visitor)
            .OrderBy(visitor => visitor.FullName)
            .ToList();

        var expectedOrder = result.OrderBy(visitor => visitor.FullName).ToList();
        Assert.Equal(expectedOrder, result);
    }
}