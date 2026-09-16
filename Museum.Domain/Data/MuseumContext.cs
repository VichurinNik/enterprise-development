using Museum.Domain.Entities;

namespace Museum.Domain.Data;

public class MuseumContext
{
    public List<Exhibition> Exhibitions { get; set; } = new();

    public List<Excursion> Excursions { get; set; } = new();

    public List<Visitor> Visitors { get; set; } = new();

    public List<Ticket> Tickets { get; set; } = new();

    public List<ExcursionExhibition> ExcursionExhibitions { get; set; } = new();
}