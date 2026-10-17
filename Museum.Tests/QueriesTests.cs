using Museum.Domain.Enums;
using Museum.Tests.Fixtures;
using Xunit;

namespace Museum.Tests;

/// <summary>
/// Содержит тесты для проверки аналитических LINQ-запросов к контексту музея.
/// Имплементация первичного конструктора (primary constructor).
/// </summary>
public class QueriesTests(MuseumFixture fixture) : IClassFixture<MuseumFixture>
{
    /// <summary>
    /// Проверяет, что запрос возвращает топ-5 выставок, отсортированных по количеству посетителей по убыванию.
    /// </summary>
    [Fact]
    public void GetTop5ExhibitionsByVisitors_WithSeededData_ReturnsOrderedExhibitions()
    {
        var context = fixture.Context;
        var expectedNames = new[]
        {
            "Статическая выставка 1",
            "Статическая выставка 10",
            "Статическая выставка 2",
            "Статическая выставка 3",
            "Статическая выставка 4"
        };

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

        Assert.Equal(expectedNames, result.Select(x => x.Exhibition.Name).ToArray());
    }

    /// <summary>
    /// Проверяет, что запрос возвращает экскурсии с минимальным количеством участников.
    /// </summary>
    [Fact]
    public void GetExcursionsWithMinimumParticipants_WithSeededData_ReturnsExcursionsWithMinimumCount()
    {
        var context = fixture.Context;
        var expectedIds = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

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

        Assert.Equal(expectedIds, result.Select(x => x.Excursion.Id).OrderBy(id => id).ToArray());
    }

    /// <summary>
    /// Проверяет, что запрос формирует корректную статистику посещаемости, сгруппированную по тематике выставки за заданный период.
    /// </summary>
    [Fact]
    public void GetAttendanceSummaryByTheme_WithinDateRange_ReturnsCorrectStatistics()
    {
        var context = fixture.Context;
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 12, 31);
        var expectedStatistics = new[]
        {
            (Theme: ExhibitionTheme.History, ExhibitionCount: 2, ExcursionCount: 4, TotalVisitors: 9, TotalRevenue: 10000m),
            (Theme: ExhibitionTheme.Art, ExhibitionCount: 3, ExcursionCount: 5, TotalVisitors: 10, TotalRevenue: 12500m),
            (Theme: ExhibitionTheme.Archaeology, ExhibitionCount: 3, ExcursionCount: 5, TotalVisitors: 10, TotalRevenue: 12500m),
            (Theme: ExhibitionTheme.Science, ExhibitionCount: 2, ExcursionCount: 4, TotalVisitors: 9, TotalRevenue: 10000m)
        };

        var result = context.ExcursionExhibitions
            .Where(x => x.Excursion != null &&
                        x.Excursion.Date.Date >= startDate &&
                        x.Excursion.Date.Date <= endDate)
            .GroupBy(x => x.Exhibition.Theme)
            .OrderBy(group => group.Key)
            .Select(group => new
            {
                Theme = group.Key,
                ExhibitionCount = group
                    .Select(x => x.ExhibitionId)
                    .Distinct()
                    .Count(),
                ExcursionCount = group
                    .Select(x => x.ExcursionId)
                    .Distinct()
                    .Count(),
                TotalVisitors = group
                    .SelectMany(x => x.Excursion?.Tickets ?? [])
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count(),
                TotalRevenue = group
                    .SelectMany(x => x.Excursion?.Tickets ?? [])
                    .DistinctBy(ticket => ticket.Id)
                    .Sum(ticket => ticket.Price)
            })
            .ToList();

        Assert.Equal(
            expectedStatistics,
            result.Select(x => (x.Theme, x.ExhibitionCount, x.ExcursionCount, x.TotalVisitors, x.TotalRevenue)).ToArray());
    }

    /// <summary>
    /// Проверяет, что возвращаются только экскурсии, проходящие в заданном зале в указанный промежуток времени.
    /// </summary>
    [Fact]
    public void GetExcursionsInSelectedHall_ForTargetHallAndDateRange_ReturnsMatchingExcursions()
    {
        var context = fixture.Context;
        const int targetHall = 1;
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 12, 31);
        var expectedIds = new[] { 1, 3, 4, 6, 7, 9 };

        var result = context.Excursions
            .Where(excursion =>
                excursion.Date.Date >= startDate &&
                excursion.Date.Date <= endDate &&
                excursion.Exhibitions.Any(x =>
                    x.Exhibition != null &&
                    x.Exhibition.HallNumber == targetHall))
            .OrderBy(excursion => excursion.Date)
            .ToList();

        Assert.Equal(expectedIds, result.Select(x => x.Id).ToArray());
    }

    /// <summary>
    /// Проверяет, что список посетителей выбранной экскурсии сортируется по алфавиту.
    /// </summary>
    [Fact]
    public void GetVisitorsForSelectedExcursion_ForTargetExcursion_ReturnsVisitorsOrderedByFullName()
    {
        var context = fixture.Context;
        const int targetExcursionId = 1;
        var expectedNames = new[]
        {
            "Посетитель Тестовый 2",
            "Посетитель Тестовый 3",
            "Посетитель Тестовый 4"
        };

        var result = context.Tickets
            .Where(t => t.ExcursionId == targetExcursionId && t.Visitor != null)
            .Select(ticket => ticket.Visitor)
            .OrderBy(visitor => visitor.FullName)
            .ToList();

        Assert.Equal(expectedNames, result.Select(x => x.FullName).ToArray());
    }
}