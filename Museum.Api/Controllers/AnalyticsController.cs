using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Analytics;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер для получения музейной аналитики и отчетов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    /// <summary>
    /// Возвращает топ-5 выставок по количеству уникальных посетителей.
    /// </summary>
    [HttpGet("top-exhibitions")]
    [ProducesResponseType(typeof(IReadOnlyList<TopExhibitionDto>), StatusCodes.Status200OK)]
    public IActionResult GetTop5Exhibitions()
    {
        var topExhibitions = analyticsService.GetTop5Exhibitions();
        return Ok(topExhibitions);
    }

    /// <summary>
    /// Возвращает сводную статистику посещаемости по тематикам выставок за указанный период.
    /// </summary>
    /// <param name="start">Дата начала периода.</param>
    /// <param name="end">Дата окончания периода.</param>
    [HttpGet("theme-summary")]
    [ProducesResponseType(typeof(IReadOnlyList<ThemeAttendanceSummaryDto>), StatusCodes.Status200OK)]
    public IActionResult GetThemeAttendanceSummary([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var summary = analyticsService.GetThemeAttendanceSummary(start, end);
        return Ok(summary);
    }
}