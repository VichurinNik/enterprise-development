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

    /// <summary>Возвращает экскурсии с минимальным количеством участников.</summary>
    public IReadOnlyList<ExcursionParticipantsDto> GetExcursionsWithMinimumParticipants();

    /// <summary>
    /// Возвращает сводную статистику посещаемости по тематикам выставок за указанный период.
    /// </summary>
    public IReadOnlyList<ThemeAttendanceSummaryDto> GetThemeAttendanceSummary(DateTime start, DateTime end);

    /// <summary>Возвращает экскурсии в выбранном зале за период.</summary>
    public IReadOnlyList<Museum.Contracts.Excursions.ExcursionsDto> GetExcursionsInHall(int hallNumber, DateTime start, DateTime end);

    /// <summary>Возвращает посетителей экскурсии в алфавитном порядке.</summary>
    public IReadOnlyList<Museum.Contracts.Visitors.VisitorDto> GetVisitorsForExcursion(int excursionId);
}
