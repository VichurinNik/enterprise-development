using Museum.Domain.Enums;

namespace Museum.Domain.Entities;

/// <summary>
/// Представляет билет на экскурсию.
/// </summary>
public class Ticket
{
    /// <summary>
    /// Уникальный идентификатор билета.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор экскурсии, на которую приобретён билет.
    /// </summary>
    public int ExcursionId { get; set; }

    /// <summary>
    /// Экскурсия, на которую приобретён билет.
    /// </summary>
    public Excursion Excursion { get; set; } = null!;

    /// <summary>
    /// Идентификатор посетителя, которому принадлежит билет.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Посетитель, которому принадлежит билет.
    /// </summary>
    public Visitor Visitor { get; set; } = null!;

    /// <summary>
    /// Тип билета.
    /// </summary>
    public TicketType TicketType { get; set; }

    /// <summary>
    /// Стоимость билета.
    /// </summary>
    public decimal Price { get; set; }
}