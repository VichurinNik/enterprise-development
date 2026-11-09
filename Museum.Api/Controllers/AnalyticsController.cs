using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Analytics;
using Museum.Contracts.Excursions;
using Museum.Contracts.Visitors;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер музейной аналитики.
/// </summary>
/// <param name="analyticsService">Сервис аналитических запросов.</param>
/// <param name="logger">Журнал запросов контроллера.</param>
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает топ-5 выставок по числу уникальных посетителей.
    /// </summary>
    /// <returns>Результаты аналитического запроса.</returns>
    [HttpGet("top-exhibitions")]
    [ProducesResponseType(typeof(IReadOnlyList<TopExhibitionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<TopExhibitionDto>> GetTop5Exhibitions()
    {
        logger.LogInformation("Выполнение аналитического запроса {Query}.", nameof(GetTop5Exhibitions));
        return Ok(analyticsService.GetTop5Exhibitions());
    }

    /// <summary>
    /// Возвращает экскурсии с минимальным количеством участников.
    /// </summary>
    /// <returns>Результаты аналитического запроса.</returns>
    [HttpGet("minimum-participants")]
    [ProducesResponseType(typeof(IReadOnlyList<ExcursionParticipantsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<ExcursionParticipantsDto>> GetExcursionsWithMinimumParticipants()
    {
        logger.LogInformation("Выполнение аналитического запроса {Query}.", nameof(GetExcursionsWithMinimumParticipants));
        return Ok(analyticsService.GetExcursionsWithMinimumParticipants());
    }

    /// <summary>
    /// Возвращает статистику по тематикам выставок за период.
    /// </summary>
    /// <param name="start">Начальная дата периода, включительно.</param>
    /// <param name="end">Конечная дата периода, включительно.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    [HttpGet("theme-summary")]
    [ProducesResponseType(typeof(IReadOnlyList<ThemeAttendanceSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<ThemeAttendanceSummaryDto>> GetThemeAttendanceSummary([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        logger.LogInformation("Выполнение аналитического запроса {Query}.", nameof(GetThemeAttendanceSummary));
        return Ok(analyticsService.GetThemeAttendanceSummary(start, end));
    }

    /// <summary>
    /// Возвращает экскурсии в выбранном зале за период.
    /// </summary>
    /// <param name="hallNumber">Номер зала.</param>
    /// <param name="start">Начальная дата периода, включительно.</param>
    /// <param name="end">Конечная дата периода, включительно.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    [HttpGet("hall-excursions")]
    [ProducesResponseType(typeof(IReadOnlyList<ExcursionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<ExcursionsDto>> GetExcursionsInHall([FromQuery] int hallNumber, [FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        logger.LogInformation("Выполнение аналитического запроса {Query}.", nameof(GetExcursionsInHall));
        return Ok(analyticsService.GetExcursionsInHall(hallNumber, start, end));
    }

    /// <summary>
    /// Возвращает посетителей экскурсии в алфавитном порядке.
    /// </summary>
    /// <param name="excursionId">Идентификатор экскурсии.</param>
    /// <returns>Результаты аналитического запроса.</returns>
    [HttpGet("excursions/{excursionId:int}/visitors")]
    [ProducesResponseType(typeof(IReadOnlyList<VisitorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<VisitorDto>> GetVisitorsForExcursion(int excursionId)
    {
        logger.LogInformation("Выполнение аналитического запроса {Query}.", nameof(GetVisitorsForExcursion));
        return Ok(analyticsService.GetVisitorsForExcursion(excursionId));
    }
}
