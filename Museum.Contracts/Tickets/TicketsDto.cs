using System.Text.Json.Serialization;
using Museum.Contracts.Excursions;
using Museum.Contracts.Visitors;
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
/// <param name="Excursion">Данные связанной экскурсии.</param>
/// <param name="Visitor">Данные связанного посетителя.</param>
/// <param name="Exhibitions">Выставки экскурсии с названиями тем.</param>
public record TicketDto
(
    int Id,
    int ExcursionId,
    int VisitorId,
    [property: JsonConverter(typeof(JsonStringEnumConverter<TicketType>))]
    TicketType TicketType,
    decimal Price,
    ExcursionsDto Excursion,
    VisitorDto Visitor,
    IReadOnlyList<TicketExhibitionDto> Exhibitions
);

/// <summary>
/// Краткие сведения о выставке, связанной с билетом через экскурсию.
/// </summary>
/// <param name="Id">Идентификатор выставки.</param>
/// <param name="Name">Название выставки.</param>
/// <param name="Theme">Название темы выставки.</param>
public record TicketExhibitionDto(int Id, string Name, string Theme);
