using Museum.Shared.Enums;

namespace Museum.Contracts.Tickets;

/// <summary>
/// DTO для создания или обновления билета на экскурсию.
/// </summary>
/// <param name="ExcursionId">Идентификатор экскурсии.</param>
/// <param name="VisitorId">Идентификатор посетителя.</param>
/// <param name="TicketType">Тип билета.</param>
/// <param name="Price">Стоимость билета.</param>
public record TicketCreateUpdateDto
    (
    int ExcursionId,
    int VisitorId,
    TicketType TicketType,
    decimal Price
    );
