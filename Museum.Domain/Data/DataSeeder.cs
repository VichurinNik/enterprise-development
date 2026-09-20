using Bogus;
using Museum.Domain.Entities;
using Museum.Domain.Enums;
using System;
using System.Linq;

namespace Museum.Domain.Data;

public static class DataSeeder
{
    public static MuseumContext Seed()
    {
        var context = new MuseumContext();

        var exhibitionFaker = new Faker<Exhibition>()
            .RuleFor(e => e.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.Name, f => $"Выставка «{f.Commerce.ProductName()}»")
            .RuleFor(e => e.Theme, f => f.PickRandom<ExhibitionTheme>())
            .RuleFor(e => e.HallNumber, f => f.Random.Int(1, 10))
            .RuleFor(e => e.StartDate, f => f.Date.Past(1))
            .RuleFor(e => e.EndDate, (f, e) => e.StartDate.AddDays(f.Random.Int(30, 180)));

        context.Exhibitions = exhibitionFaker.Generate(20); 

        var excursionFaker = new Faker<Excursion>()
            .RuleFor(e => e.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.Date, f => f.Date.Between(DateTime.Today.AddMonths(-2), DateTime.Today.AddMonths(2)))
            .RuleFor(e => e.StartTime, f => TimeSpan.FromHours(f.Random.Int(9, 18)))
            .RuleFor(e => e.Duration, f => TimeSpan.FromMinutes(f.Random.Int(45, 120)));

        context.Excursions = excursionFaker.Generate(20); 

        var visitorFaker = new Faker<Visitor>()
            .RuleFor(v => v.Id, f => f.IndexFaker + 1)
            .RuleFor(v => v.FullName, f => f.Name.FullName())
            .RuleFor(v => v.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(v => v.BirthDate, f => f.Date.Past(60, DateTime.Today.AddYears(-18)));

        context.Visitors = visitorFaker.Generate(20); 

        var random = new Random();

        foreach (var excursion in context.Excursions)
        {
            var exhibitionsCount = random.Next(2, 5); 
            var selectedExhibitions = context.Exhibitions.OrderBy(_ => random.Next()).Take(exhibitionsCount).ToList();

            foreach (var exhibition in selectedExhibitions)
            {
                var relation = new ExcursionExhibition
                {
                    ExcursionId = excursion.Id,
                    Excursion = excursion,
                    ExhibitionId = exhibition.Id,
                    Exhibition = exhibition
                };

                context.ExcursionExhibitions.Add(relation);
                excursion.Exhibitions.Add(relation);
                exhibition.ExcursionExhibitions.Add(relation);
            }
        }

        var ticketId = 1;

        foreach (var excursion in context.Excursions)
        {
            var participantsCount = random.Next(3, 11); 
            var selectedVisitors = context.Visitors.OrderBy(_ => random.Next()).Take(participantsCount).ToList();

            foreach (var visitor in selectedVisitors)
            {
                var ticketType = random.Next(2) == 0 ? TicketType.Adult : TicketType.Discounted;
                var price = ticketType == TicketType.Adult ? random.Next(500, 1001) : random.Next(200, 501);

                var ticket = new Ticket
                {
                    Id = ticketId++,
                    ExcursionId = excursion.Id,
                    Excursion = excursion,
                    VisitorId = visitor.Id,
                    Visitor = visitor,
                    TicketType = ticketType,
                    Price = price
                };

                context.Tickets.Add(ticket);
                excursion.Tickets.Add(ticket);
                visitor.Tickets.Add(ticket);
            }
        }

        return context;
    }
}