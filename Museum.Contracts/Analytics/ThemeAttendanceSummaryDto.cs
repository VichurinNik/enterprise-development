using Museum.Shared.Enums;

namespace Museum.Contracts.Analytics;

/// <summary>
/// Статистика посещаемости по тематике выставок за период.
/// </summary>
/// <param name="Theme">Тематика выставок.</param>
/// <param name="ExhibitionCount">Количество выставок.</param>
/// <param name="ExcursionCount">Количество экскурсий.</param>
/// <param name="TotalVisitors">Количество уникальных посетителей.</param>
/// <param name="TotalRevenue">Общая стоимость билетов.</param>
public record ThemeAttendanceSummaryDto(
    ExhibitionTheme Theme,
    int ExhibitionCount,
    int ExcursionCount,
    int TotalVisitors,
    decimal TotalRevenue);
