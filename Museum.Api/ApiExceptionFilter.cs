using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Museum.Api;

/// <summary>Единая обработка ошибок для всех контроллеров API.</summary>
public sealed class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    /// <summary>Записывает ошибку в журнал и возвращает HTTP-ответ с её описанием.</summary>
    /// <param name="context">Контекст ошибки запроса.</param>
    public void OnException(ExceptionContext context)
    {
        var invalidInput = context.Exception is ArgumentException or KeyNotFoundException;
        logger.LogError(context.Exception, "Ошибка при обработке запроса {Path}.", context.HttpContext.Request.Path);
        var status = invalidInput ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = invalidInput ? "Некорректный запрос" : "Внутренняя ошибка сервера",
            Detail = invalidInput ? context.Exception.Message : "Не удалось обработать запрос."
        }) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
