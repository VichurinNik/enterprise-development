namespace Museum.Contracts.Excursions;

/// <summary>
/// DTO для создания или обновления экскурсии.
/// </summary>
public record ExcursionCreateUpdateDto(
    DateTime Date,
    TimeSpan Duration,
    IReadOnlyList<int> ExhibitionIds);