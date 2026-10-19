using Museum.Domain.Enums;

namespace Museum.Contracts.Analytics;

/// <summary>
/// DTO выставки с количеством уникальных посетителей.
/// </summary>
public record TopExhibitionDto(
    int ExhibitionId,
    string Name,
    ExhibitionTheme Theme,
    int HallNumber,
    int VisitorCount);