using System;
using System.Collections.Generic;

namespace Museum.Domain.Entities;

public class Excursion
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan Duration { get; set; }

    public List<ExcursionExhibition> Exhibitions { get; set; } = new();

    public List<Ticket> Tickets { get; set; } = new();
}