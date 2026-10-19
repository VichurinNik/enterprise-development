using Museum.Domain.Enums;

namespace Museum.Contracts.Tickets;

/// <summary>
/// DTO для создания или обновления билета на экскурсию.
/// </summary>
public record TicketCreateUpdateDto
    (
    int ExcursionId,
    int VisitorId,
    TicketType TicketType,
    decimal Price
    );