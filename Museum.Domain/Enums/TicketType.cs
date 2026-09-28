namespace Museum.Domain.Enums;

/// <summary>
/// Тип билета для посетителя.
/// </summary>
public enum TicketType
{
    /// <summary>
    /// Взрослый билет за полную стоимость.
    /// </summary>
    Adult,

    /// <summary>
    /// Льготный билет (для детей, студентов, пенсионеров)..
    /// </summary>
    Discounted
}
