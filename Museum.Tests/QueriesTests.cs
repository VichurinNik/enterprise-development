using System;
using System.Linq;
using Museum.Domain.Enums;
using Museum.Tests.Fixtures;
using Xunit;

namespace Museum.Tests;

public class QueriesTests : IClassFixture<MuseumFixture>
{
    private readonly MuseumFixture _fixture;

    public QueriesTests(MuseumFixture fixture)
    {
        _fixture = fixture;
    }

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

        for (var i = 1; i < result.Count; i++)
        {
            Assert.True(result[i - 1].VisitorCount >= result[i].VisitorCount);
        }
    }

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

    [Fact]
    public void GetExcursionsInSelectedHall_ShouldReturnCorrectResult()
    {
        var context = _fixture.Context;

        var selectedExhibition = context.Exhibitions.FirstOrDefault(e => e.ExcursionExhibitions.Any());
        Assert.NotNull(selectedExhibition);

        var selectedHall = selectedExhibition.HallNumber;

        var dates = context.Excursions.Select(x => x.Date.Date).ToList();
        Assert.NotEmpty(dates);

        var startDate = dates.Min();
        var endDate = dates.Max();

        var result = context.Excursions
            .Where(excursion =>
                excursion.Date.Date >= startDate &&
                excursion.Date.Date <= endDate &&
                excursion.Exhibitions.Any(x => x.Exhibition != null && x.Exhibition.HallNumber == selectedHall))
            .OrderBy(excursion => excursion.Date)
            .ThenBy(excursion => excursion.StartTime)
            .ToList();

        Assert.NotEmpty(result);
        Assert.All(result, excursion =>
        {
            Assert.True(excursion.Date.Date >= startDate);
            Assert.True(excursion.Date.Date <= endDate);
            Assert.Contains(excursion.Exhibitions, relation => relation.Exhibition?.HallNumber == selectedHall);
        });
    }

    [Fact]
    public void GetVisitorsForSelectedExcursion_ShouldReturnOrderedResult()
    {
        var context = _fixture.Context;

        var selectedExcursion = context.Excursions.FirstOrDefault();
        Assert.NotNull(selectedExcursion);

        var result = (selectedExcursion.Tickets ?? new())
            .Where(ticket => ticket.Visitor != null)
            .Select(ticket => ticket.Visitor)
            .OrderBy(visitor => visitor.FullName)
            .ToList();

        for (var i = 1; i < result.Count; i++)
        {
            Assert.True(string.Compare(result[i - 1].FullName, result[i].FullName, StringComparison.Ordinal) <= 0);
        }
    }
}