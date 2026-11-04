namespace Museum.Contracts.Excursions;

/// <summary>
/// DTO для создания или обновления экскурсии.
/// </summary>
/// <param name="Date">Дата и время начала.</param>
/// <param name="Duration">Продолжительность.</param>
/// <param name="ExhibitionIds">Идентификаторы выставок.</param>
public record ExcursionCreateUpdateDto(
    DateTime Date,
    TimeSpan Duration,
    IReadOnlyList<int> ExhibitionIds);
