namespace Museum.Domain.Entities;

/// <summary>
/// Представляет связь между экскурсией и выставкой.
/// </summary>
public class ExcursionExhibition
{
    /// <summary>
    /// Идентификатор экскурсии.
    /// </summary>
    public int ExcursionId { get; set; }

    /// <summary>
    /// Экскурсия, связанная с выставкой.
    /// </summary>
    public Excursion Excursion { get; set; } = null!;

    /// <summary>
    /// Идентификатор выставки.
    /// </summary>
    public int ExhibitionId { get; set; }

    /// <summary>
    /// Выставка, посещаемая во время экскурсии.
    /// </summary>
    public Exhibition Exhibition { get; set; } = null!;
}