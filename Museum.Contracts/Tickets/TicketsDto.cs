using Museum.Domain.Enums;

namespace Museum.Contracts.Tickets;
/// <summary>
/// DTO для возврата сведений о билете на экскурсию.
/// </summary>
public record TicketDto
(
    int Id,
    int ExcursionId,
    int VisitorId,
    TicketType TicketType,
    decimal Price
);
