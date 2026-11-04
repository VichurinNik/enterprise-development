using Museum.Shared.Enums;

namespace Museum.Contracts.Analytics;

/// <summary>
/// DTO выставки с количеством уникальных посетителей.
/// </summary>
/// <param name="ExhibitionId">Идентификатор выставки.</param>
/// <param name="Name">Название выставки.</param>
/// <param name="Theme">Тематика выставки.</param>
/// <param name="HallNumber">Номер зала.</param>
/// <param name="VisitorCount">Количество уникальных посетителей.</param>
public record TopExhibitionDto(
    int ExhibitionId,
    string Name,
    ExhibitionTheme Theme,
    int HallNumber,
    int VisitorCount);
