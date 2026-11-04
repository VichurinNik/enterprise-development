using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Analytics;
using Museum.Contracts.Excursions;
using Museum.Contracts.Visitors;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер для получения музейной аналитики и отчетов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает топ-5 выставок по количеству уникальных посетителей.
    /// </summary>
    [HttpGet("top-exhibitions")]
    [ProducesResponseType(typeof(IReadOnlyList<TopExhibitionDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<TopExhibitionDto>> GetTop5Exhibitions()
    {
        logger.LogInformation("Запрошен топ выставок.");
        var topExhibitions = analyticsService.GetTop5Exhibitions();
        return Ok(topExhibitions);
    }

    /// <summary>Возвращает экскурсии с наименьшим числом участников.</summary>
    [HttpGet("minimum-participants")]
    [ProducesResponseType(typeof(IReadOnlyList<ExcursionParticipantsDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ExcursionParticipantsDto>> GetExcursionsWithMinimumParticipants()
    {
        logger.LogInformation("Запрошены экскурсии с минимальным количеством участников.");
        return Ok(analyticsService.GetExcursionsWithMinimumParticipants());
    }

    /// <summary>
    /// Возвращает сводную статистику посещаемости по тематикам выставок за указанный период.
    /// </summary>
    /// <param name="start">Дата начала периода.</param>
    /// <param name="end">Дата окончания периода.</param>
    [HttpGet("theme-summary")]
    [ProducesResponseType(typeof(IReadOnlyList<ThemeAttendanceSummaryDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ThemeAttendanceSummaryDto>> GetThemeAttendanceSummary([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        logger.LogInformation("Запрошена статистика за период {Start}–{End}.", start, end);
        var summary = analyticsService.GetThemeAttendanceSummary(start, end);
        return Ok(summary);
    }

    /// <summary>Возвращает экскурсии в зале за указанный период.</summary>
    [HttpGet("hall-excursions")]
    [ProducesResponseType(typeof(IReadOnlyList<ExcursionsDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ExcursionsDto>> GetExcursionsInHall(
        [FromQuery] int hallNumber, [FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        logger.LogInformation("Запрошены экскурсии в зале {HallNumber}.", hallNumber);
        return Ok(analyticsService.GetExcursionsInHall(hallNumber, start, end));
    }

    /// <summary>Возвращает посетителей выбранной экскурсии по алфавиту.</summary>
    [HttpGet("excursions/{excursionId:int}/visitors")]
    [ProducesResponseType(typeof(IReadOnlyList<VisitorDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<VisitorDto>> GetVisitorsForExcursion(int excursionId)
    {
        logger.LogInformation("Запрошены посетители экскурсии {ExcursionId}.", excursionId);
        return Ok(analyticsService.GetVisitorsForExcursion(excursionId));
    }
}
