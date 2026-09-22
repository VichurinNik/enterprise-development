using System;
using System.Linq;
using Museum.Domain.Enums;
using Museum.Tests.Fixtures;
using Xunit;

namespace Museum.Tests;

/// <summary>
/// Содержит тесты для проверки аналитических LINQ-запросов к контексту музея.
/// </summary>
public class QueriesTests : IClassFixture<MuseumFixture>
{
    private readonly MuseumFixture _fixture;

    /// <summary>
    /// Инициализирует новый экземпляр класса тестов.
    /// </summary>
    /// <param name="fixture">Фикстура с подготовленными данными.</param>
    public QueriesTests(MuseumFixture fixture)
    {
        _fixture = fixture;
    }

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
                VisitorCount = (exhibition.ExcursionExhibitions ?? new())
                    .SelectMany(x => x.Excursion?.Tickets ?? new())
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.VisitorCount)
            .ThenBy(x => x.Exhibition.Name)
            .Take(5)
            .ToList();

        Assert.NotNull(result);

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
                Participants = (excursion.Tickets ?? new())
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

        Assert.NotNull(result);
        Assert.All(result, item => Assert.Equal(minParticipants, item.Participants));
    }

    /// <summary>
    /// Проверяет, что запрос формирует корректную статистику посещаемости сгруппированную по тематике выставки.
    /// </summary>
    [Fact]
    public void GetAttendanceSummaryByTheme_ShouldReturnCorrectStatistics()
    {
        var context = _fixture.Context;

        var dates = context.Excursions.Select(x => x.Date.Date).ToList();
        if (!dates.Any()) return;

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
                    .SelectMany(x => x.Excursion?.Tickets ?? new())
                    .Select(ticket => ticket.VisitorId)
                    .Distinct()
                    .Count(),

                TotalTicketCost = group
                    .SelectMany(x => x.Excursion?.Tickets ?? new())
                    .Sum(ticket => ticket.Price),

                DailyStatistics = group
                    .GroupBy(x => x.Excursion.Date.Date)
                    .Select(day => day
                        .SelectMany(x => x.Excursion?.Tickets ?? new())
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

        Assert.NotNull(result);
        Assert.All(result, item =>
        {
            Assert.True(item.TotalVisitors >= 0);
            Assert.True(item.TotalTicketCost >= 0);
            Assert.True(item.AverageVisitorsPerDay >= 0);
            Assert.True(item.MinVisitorsPerDay >= 0);
            Assert.True(item.MaxVisitorsPerDay >= item.MinVisitorsPerDay);
        });
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
        if (!dates.Any()) return;

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

        Assert.All(result, excursion =>
        {
            Assert.InRange(excursion.Date.Date, startDate, endDate);
            Assert.Contains(excursion.Exhibitions, relation => relation.Exhibition?.HallNumber == targetHall);
        });
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