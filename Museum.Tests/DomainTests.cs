using Museum.Domain.Entities;
using Museum.Domain.Enums;
namespace Museum.Tests;

public class DomainTests
{
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
    }

    [Fact]
    public void Ticket_ShouldHaveCorrectPriceAndType()
    {
        var ticket = new Ticket
        {
            Id = 1,
            TicketType = TicketType.Adult,
            Price = 1000
        };

        Assert.Equal(TicketType.Adult, ticket.TicketType);
        Assert.Equal(1000, ticket.Price);
    }

    [Fact]
    public void Visitor_ShouldHaveFullName()
    {
        var visitor = new Visitor
        {
            Id = 1,
            FullName = "Иван Иванов",
            Phone = "+79990000000",
            BirthDate = new DateTime(2000, 1, 1)
        };

        Assert.Equal("Иван Иванов", visitor.FullName);
        Assert.Equal("+79990000000", visitor.Phone);
    }
}