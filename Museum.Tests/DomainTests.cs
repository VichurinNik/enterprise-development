using Museum.Domain.Entities;
using Museum.Domain.Enums;

namespace Museum.Tests;

/// <summary>
/// Проверяет корректность основных классов предметной области музея.
/// </summary>
public class DomainTests
{
    /// <summary>
    /// Проверяет свойства музейной выставки.
    /// </summary>
    [Fact]
    public void Exhibition_ShouldHaveCorrectProperties()
    {
        var exhibition = new Exhibition
        {
            Id = 1,
            Name = "Древняя история",
            Theme = ExhibitionTheme.History,
            HallNumber = 1,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1)
        };

        Assert.Equal(1, exhibition.Id);
        Assert.Equal("Древняя история", exhibition.Name);
        Assert.Equal(ExhibitionTheme.History, exhibition.Theme);
        Assert.Equal(1, exhibition.HallNumber);
        Assert.Equal(
            new DateTime(2026, 1, 1),
            exhibition.StartDate);
        Assert.Equal(
            new DateTime(2026, 6, 1),
            exhibition.EndDate);
        Assert.NotNull(exhibition.ExcursionExhibitions);
    }

    /// <summary>
    /// Проверяет свойства музейного билета.
    /// </summary>
    [Fact]
    public void Ticket_ShouldHaveCorrectProperties()
    {
        var ticket = new Ticket
        {
            Id = 1,
            ExcursionId = 10,
            VisitorId = 5,
            TicketType = TicketType.Adult,
            Price = 1000m
        };

        Assert.Equal(1, ticket.Id);
        Assert.Equal(10, ticket.ExcursionId);
        Assert.Equal(5, ticket.VisitorId);
        Assert.Equal(TicketType.Adult, ticket.TicketType);
        Assert.Equal(1000m, ticket.Price);
    }

    /// <summary>
    /// Проверяет свойства посетителя музея.
    /// </summary>
    [Fact]
    public void Visitor_ShouldHaveCorrectProperties()
    {
        var visitor = new Visitor
        {
            Id = 1,
            FullName = "Иван Иванов",
            Phone = "+79990000000",
            BirthDate = new DateTime(2000, 1, 1)
        };

        Assert.Equal(1, visitor.Id);
        Assert.Equal("Иван Иванов", visitor.FullName);
        Assert.Equal("+79990000000", visitor.Phone);
        Assert.Equal(
            new DateTime(2000, 1, 1),
            visitor.BirthDate);
        Assert.NotNull(visitor.Tickets);
    }
}