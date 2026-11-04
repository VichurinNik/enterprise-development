
namespace Museum.Contracts.Excursions;
/// <summary>
/// DTO для возврата сведений об экскурсии.
/// </summary>
/// <param name="Id">Идентификатор экскурсии.</param>
/// <param name="Date">Дата и время начала.</param>
/// <param name="Duration">Продолжительность.</param>
/// <param name="ExhibitionIds">Идентификаторы выставок.</param>
public record ExcursionsDto(
    int Id,
    DateTime Date,
    TimeSpan Duration,
    IReadOnlyList<int> ExhibitionIds
);
