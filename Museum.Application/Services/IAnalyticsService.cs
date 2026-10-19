using Museum.Contracts.Analytics;

namespace Museum.Application.Services;

/// <summary>
/// Интерфейс службы для получения аналитической информации.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Возвращает топ-5 выставок по количеству посетителей.
    /// </summary>
    public IReadOnlyList<TopExhibitionDto> GetTop5Exhibitions();

    /// <summary>
    /// Возвращает сводную статистику посещаемости по тематикам выставок за указанный период.
    /// </summary>
    public IReadOnlyList<ThemeAttendanceSummaryDto> GetThemeAttendanceSummary(DateTime start, DateTime end);
}