using Museum.Domain.Enums;
using Museum.Tests.Fixtures;
using Xunit;

namespace Museum.Tests;

/// <summary>
/// Содержит тесты для проверки аналитических LINQ-запросов к контексту музея..
/// Имплементация первичного конструктора (primary constructor).
/// </summary>
public class QueriesTests(MuseumFixture fixture) : IClassFixture<MuseumFixture>
{
    /// <summary>
    /// Проверяет, что запрос возвращает топ-5 выставок, отсортированных по количеству посетителей по убыванию.
    /// </summary>
    [Fact]
    public void GetTop5ExhibitionsByVisitors_ShouldReturnOrderedResult()
    {
        var context = fixture.Context;

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

        var expectedNames = new[]
        {
            "Статическая выставка 1",
            "Статическая выставка 10",
            "Статическая выставка 2",
            "Статическая выставка 3",
            "Статическая выставка 4"
        };

        Assert.Equal(expectedNames, result.Select(x => x.Exhibition.Name).ToArray());
    }

    /// <summary>
    /// Проверяет, что запрос возвращает экскурсии с минимальным количеством участников.
    /// </summary>
    [Fact]
    public void GetExcursionsWithMinimumParticipants_ShouldReturnCorrectResult()
    {
        var context = fixture.Context;

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

        var expectedIds = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        Assert.Equal(expectedIds, result.Select(x => x.Excursion.Id).OrderBy(id => id).ToArray());
    }

    /// <summary>
    /// Проверяет, что запрос формирует корректную статистику посещаемости сгруппированную по тематике выставки.
    /// </summary>
    [Fact]
    public void GetAttendanceSummaryByTheme_ShouldReturnCorrectStatistics()
    {
        var context = fixture.Context;

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
                    .Count()
            })
            .ToList();

        var expectedThemes = new[]
        {
            ExhibitionTheme.History,
            ExhibitionTheme.Art,
            ExhibitionTheme.Archaeology,
            ExhibitionTheme.Science
        };

        Assert.Equal(expectedThemes.OrderBy(t => t).ToArray(), result.Select(x => x.Theme).OrderBy(t => t).ToArray());
    }

    /// <summary>
    /// Проверяет, что возвращаются только экскурсии, проходящие в заданном зале (используется константа).
    /// </summary>
    [Fact]
    public void GetExcursionsInSelectedHall_ShouldReturnCorrectResult()
    {
        var context = fixture.Context;
        const int targetHall = 1;

        var dates = context.Excursions.Select(x => x.Date.Date).ToList();
        if (dates.Count == 0) return;

        var startDate = dates.Min();
        var endDate = dates.Max();

        var result = context.Excursions
            .Where(excursion =>
                excursion.Date.Date >= startDate &&
                excursion.Date.Date <= endDate &&
                excursion.Exhibitions.Any(x =>
                    x.Exhibition != null &&
                    x.Exhibition.HallNumber == targetHall))
            .OrderBy(excursion => excursion.Date)
            .ToList();

        var expectedIds = new[] { 2, 3, 5, 6, 8, 9 };

        Assert.Equal(
            expectedIds,
            result.Select(x => x.Id).ToArray());
    }
    /// <summary>
    /// Проверяет, что список посетителей выбранной экскурсии сортируется по алфавиту.
    /// </summary>
    [Fact]
    public void GetVisitorsForSelectedExcursion_ShouldReturnOrderedResult()
    {
        var context = fixture.Context;
        const int targetExcursionId = 1;

        var result = context.Tickets
            .Where(t => t.ExcursionId == targetExcursionId && t.Visitor != null)
            .Select(ticket => ticket.Visitor)
            .OrderBy(visitor => visitor.FullName)
            .ToList();

        var expectedNames = new[]
        {
            "Посетитель Тестовый 2",
            "Посетитель Тестовый 3",
            "Посетитель Тестовый 4"
        };

        Assert.Equal(expectedNames, result.Select(x => x.FullName).ToArray());
    }
}
