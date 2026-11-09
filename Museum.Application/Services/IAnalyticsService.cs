using Museum.Contracts.Analytics;
using Museum.Contracts.Excursions;
using Museum.Contracts.Visitors;

namespace Museum.Application.Services;

/// <summary>
/// Сервис аналитических запросов к данным музея.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Возвращает топ-5 выставок по числу уникальных посетителей.
    /// </summary>
    /// <returns>Результаты аналитического запроса.</returns>
    IReadOnlyList<TopExhibitionDto> GetTop5Exhibitions();

    /// <summary>
    /// Возвращает экскурсии с минимальным количеством участников.
    /// </summary>
    /// <returns>Результаты аналитического запроса.</returns>
    IReadOnlyList<ExcursionParticipantsDto> GetExcursionsWithMinimumParticipants();

    /// <summary>
    /// Возвращает статистику по тематикам выставок за период.
    /// </summary>
    /// <param name="start">Начальная дата периода, включительно.</param>
    /// <param name="end">Конечная дата периода, включительно.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    IReadOnlyList<ThemeAttendanceSummaryDto> GetThemeAttendanceSummary(DateTime start, DateTime end);

    /// <summary>
    /// Возвращает экскурсии в выбранном зале за период.
    /// </summary>
    /// <param name="hallNumber">Номер зала.</param>
    /// <param name="start">Начальная дата периода, включительно.</param>
    /// <param name="end">Конечная дата периода, включительно.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    IReadOnlyList<ExcursionsDto> GetExcursionsInHall(int hallNumber, DateTime start, DateTime end);

    /// <summary>
    /// Возвращает посетителей экскурсии в алфавитном порядке.
    /// </summary>
    /// <param name="excursionId">Идентификатор экскурсии.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    IReadOnlyList<VisitorDto> GetVisitorsForExcursion(int excursionId);
}
