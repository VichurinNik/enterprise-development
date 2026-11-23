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
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    public Task<IReadOnlyList<TopExhibitionDto>> GetTop5ExhibitionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает экскурсии с минимальным количеством участников.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    public Task<IReadOnlyList<ExcursionParticipantsDto>> GetExcursionsWithMinimumParticipantsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает статистику по тематикам выставок за период.
    /// </summary>
    /// <param name="start">Начальная дата периода, включительно.</param>
    /// <param name="end">Конечная дата периода, включительно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    public Task<IReadOnlyList<ThemeAttendanceSummaryDto>> GetThemeAttendanceSummaryAsync(DateTime start, DateTime end, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает экскурсии в выбранном зале за период.
    /// </summary>
    /// <param name="hallNumber">Номер зала.</param>
    /// <param name="start">Начальная дата периода, включительно.</param>
    /// <param name="end">Конечная дата периода, включительно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    public Task<IReadOnlyList<ExcursionsDto>> GetExcursionsInHallAsync(int hallNumber, DateTime start, DateTime end, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает посетителей экскурсии в алфавитном порядке.
    /// </summary>
    /// <param name="excursionId">Идентификатор экскурсии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    public Task<IReadOnlyList<VisitorDto>> GetVisitorsForExcursionAsync(int excursionId, CancellationToken cancellationToken);
}
