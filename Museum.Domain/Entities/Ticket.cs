using Museum.Domain.Enums;

namespace Museum.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }

    public int ExcursionId { get; set; }

    public Excursion Excursion { get; set; } = null!;

    public int VisitorId { get; set; }

    public Visitor Visitor { get; set; } = null!;

    public TicketType TicketType { get; set; }

    public decimal Price { get; set; }
}