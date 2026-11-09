using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Museum.Application.Exceptions;

namespace Museum.Api;

/// <summary>
/// Преобразует ошибки сервисов в HTTP-ответы контроллеров.
/// </summary>
/// <param name="logger">Журнал ошибок приложения.</param>
public sealed class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    /// <summary>
    /// Записывает ошибку в журнал и возвращает ответ с подходящим HTTP-кодом.
    /// </summary>
    /// <param name="context">Контекст исключения, возникшего при выполнении запроса.</param>
    public void OnException(ExceptionContext context)
    {
        var (status, title) = context.Exception switch
        {
            EntityInUseException => (StatusCodes.Status409Conflict, "Запись используется"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Некорректный запрос"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Запись не найдена"),
            _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(context.Exception, "Ошибка обработки запроса {Path}.", context.HttpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(context.Exception, "Запрос {Path} отклонён.", context.HttpContext.Request.Path);
        }

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError
                ? "Не удалось обработать запрос."
                : context.Exception.Message
        }) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
