using Museum.Domain.Entities;
using Museum.Domain.Enums;

namespace Museum.Domain.Data;

public static class DataSeeder
{
    public static MuseumContext Seed()
    {
        var context = new MuseumContext();

        var visitor1 = new Visitor { Id = 1, FullName = "Иванов Иван" };
        var visitor2 = new Visitor { Id = 2, FullName = "Петров Петр" };
        context.Visitors.AddRange(new[] { visitor1, visitor2 });

        var exhibition = new Exhibition
        {
            Id = 1,
            Name = "Шедевры Живописи",
            Theme = ExhibitionTheme.Art,
            HallNumber = 1
        };
        context.Exhibitions.Add(exhibition);

        var excursion = new Excursion
        {
            Id = 1,
            Date = DateTime.Now,
            StartTime = new TimeSpan(10, 0, 0),
            Tickets = new List<Ticket>()
        };

        var excursionExhibition = new ExcursionExhibition
        {
            Excursion = excursion,
            Exhibition = exhibition
        };
        excursion.Exhibitions = new List<ExcursionExhibition> { excursionExhibition };
        exhibition.ExcursionExhibitions = new List<ExcursionExhibition> { excursionExhibition };
        context.ExcursionExhibitions.Add(excursionExhibition);

        var ticket1 = new Ticket { Id = 1, ExcursionId = 1, VisitorId = 1, Visitor = visitor1, Price = 500 };
        var ticket2 = new Ticket { Id = 2, ExcursionId = 1, VisitorId = 2, Visitor = visitor2, Price = 500 };

        excursion.Tickets.Add(ticket1);
        excursion.Tickets.Add(ticket2);

        context.Tickets.AddRange(new[] { ticket1, ticket2 });
        context.Excursions.Add(excursion);

        return context;
    }
}