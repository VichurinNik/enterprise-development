using Museum.Domain.Entities;

namespace Museum.Domain.Data;

/// <summary>
/// Контекст данных предметной области музея.
/// Содержит коллекции всех основных сущностей и связей.
/// </summary>
public class MuseumContext
{
    /// <summary>
    /// Коллекция музейных выставок.
    /// </summary>
    public List<Exhibition> Exhibitions { get; set; } = new();

    /// <summary>
    /// Коллекция музейных экскурсий.
    /// </summary>
    public List<Excursion> Excursions { get; set; } = new();

    /// <summary>
    /// Коллекция посетителей музея.
    /// </summary>
    public List<Visitor> Visitors { get; set; } = new();

    /// <summary>
    /// Коллекция билетов на экскурсии.
    /// </summary>
    public List<Ticket> Tickets { get; set; } = new();

    /// <summary>
    /// Коллекция связей между экскурсиями и выставками.
    /// </summary>
    public List<ExcursionExhibition> ExcursionExhibitions { get; set; } = new();
}