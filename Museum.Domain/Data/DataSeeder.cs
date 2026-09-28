using Museum.Domain.Entities;
using Museum.Domain.Enums;

namespace Museum.Domain.Data;

/// <summary>
/// Отвечает за генерацию начальных статических данных для предметной области музея.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Создает и заполняет контекст музея статичными детерминированными данными.
    /// </summary>
    /// <returns>Заполненный контекст <see cref="MuseumContext"/>.</returns>
    public static MuseumContext Seed()
    {
        var context = new MuseumContext();

        for (var i = 1; i <= 10; i++)
        {
            context.Exhibitions.Add(new Exhibition
            {
                Id = i,
                Name = $"Статическая выставка {i}",
                Theme = (ExhibitionTheme)(i % 4),
                HallNumber = (i % 3) + 1,
                StartDate = new DateTime(2026, 1, 1).AddMonths(i),
                EndDate = new DateTime(2026, 2, 1).AddMonths(i)
            });
        }

        for (var i = 1; i <= 10; i++)
        {
            context.Excursions.Add(new Excursion
            {
                Id = i,
                Date = new DateTime(2026, 1, 15, 10 + (i % 5), 0, 0).AddDays(i * 2),
                Duration = TimeSpan.FromHours(1.5)
            });
        }

        for (var i = 1; i <= 10; i++)
        {
            context.Visitors.Add(new Visitor
            {
                Id = i,
                FullName = $"Посетитель Тестовый {i}",
                Phone = $"+790000000{i:D2}",
                BirthDate = new DateOnly(1990 + (i % 20), (i % 12) + 1, 15)
            });
        }

        for (var i = 1; i <= 10; i++)
        {
            var exhibition1 = context.Exhibitions[i - 1];
            var exhibition2 = context.Exhibitions[(i + 1) % 10];
            var excursion = context.Excursions[i - 1];

            var rel1 = new ExcursionExhibition { ExcursionId = excursion.Id, Excursion = excursion, ExhibitionId = exhibition1.Id, Exhibition = exhibition1 };
            var rel2 = new ExcursionExhibition { ExcursionId = excursion.Id, Excursion = excursion, ExhibitionId = exhibition2.Id, Exhibition = exhibition2 };

            context.ExcursionExhibitions.Add(rel1);
            context.ExcursionExhibitions.Add(rel2);
            excursion.Exhibitions.Add(rel1);
            excursion.Exhibitions.Add(rel2);
            exhibition1.ExcursionExhibitions.Add(rel1);
            exhibition2.ExcursionExhibitions.Add(rel2);
        }

        var ticketId = 1;
        for (var i = 1; i <= 10; i++)
        {
            var excursion = context.Excursions[i - 1];

            for (var j = 0; j < 3; j++)
            {
                var visitor = context.Visitors[(i + j) % 10];
                var ticketType = j % 2 == 0 ? TicketType.Adult : TicketType.Discounted;

                var ticket = new Ticket
                {
                    Id = ticketId++,
                    ExcursionId = excursion.Id,
                    Excursion = excursion,
                    VisitorId = visitor.Id,
                    Visitor = visitor,
                    TicketType = ticketType,
                    Price = ticketType == TicketType.Adult ? 1000m : 500m
                };

                context.Tickets.Add(ticket);
                excursion.Tickets.Add(ticket);
                visitor.Tickets.Add(ticket);
            }
        }

        return context;
    }
}