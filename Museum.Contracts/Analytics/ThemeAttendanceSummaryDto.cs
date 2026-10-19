using Museum.Domain.Enums;

namespace Museum.Contracts.Analytics;

/// <summary>
/// DTO сводной статистики посещаемости по тематике выставок за период.
/// </summary>
public record ThemeAttendanceSummaryDto(
    ExhibitionTheme Theme,
    int TotalVisitors,
    decimal TotalTicketCost,
    double AverageVisitorsPerDay,
    int MinVisitorsPerDay,
    int MaxVisitorsPerDay);