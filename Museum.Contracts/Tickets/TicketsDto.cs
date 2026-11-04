using Museum.Shared.Enums;

namespace Museum.Contracts.Tickets;
/// <summary>
/// DTO для возврата сведений о билете на экскурсию.
/// </summary>
/// <param name="Id">Идентификатор билета.</param>
/// <param name="ExcursionId">Идентификатор экскурсии.</param>
/// <param name="VisitorId">Идентификатор посетителя.</param>
/// <param name="TicketType">Тип билета.</param>
/// <param name="Price">Стоимость билета.</param>
public record TicketDto
(
    int Id,
    int ExcursionId,
    int VisitorId,
    TicketType TicketType,
    decimal Price
);
