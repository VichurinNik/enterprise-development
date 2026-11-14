using Microsoft.EntityFrameworkCore;
using Museum.Domain.Data;
using Museum.Domain.Entities;

namespace Museum.Infrastructure.MySql;

/// <summary>
/// Описывает начальные записи, включаемые в миграции базы музея.
/// </summary>
internal static class MuseumSeedConfiguration
{
    internal static void Apply(ModelBuilder modelBuilder)
    {
        var data = DataSeeder.Seed();
        modelBuilder.Entity<Exhibition>().HasData(data.Exhibitions.Select(item => new
        {
            item.Id, item.Name, item.Theme, item.HallNumber, item.StartDate, item.EndDate
        }));
        modelBuilder.Entity<Excursion>().HasData(data.Excursions.Select(item => new
        {
            item.Id, item.Date, item.Duration
        }));
        modelBuilder.Entity<Visitor>().HasData(data.Visitors.Select(item => new
        {
            item.Id, item.FullName, item.Phone, item.BirthDate
        }));
        modelBuilder.Entity<Ticket>().HasData(data.Tickets.Select(item => new
        {
            item.Id, item.ExcursionId, item.VisitorId, item.TicketType, item.Price
        }));
        modelBuilder.Entity<ExcursionExhibition>().HasData(data.ExcursionExhibitions.Select(item => new
        {
            item.ExcursionId, item.ExhibitionId
        }));
    }
}
