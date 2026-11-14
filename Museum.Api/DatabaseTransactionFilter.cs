using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Museum.Infrastructure.MySql;

namespace Museum.Api;

/// <summary>
/// Объединяет изменения одного HTTP-запроса в транзакцию базы данных.
/// </summary>
/// <param name="database">Контекст базы данных текущего запроса.</param>
public sealed class DatabaseTransactionFilter(MuseumDbContext database) : IAsyncActionFilter
{
    /// <summary>
    /// Подтверждает изменения только после успешного выполнения действия контроллера.
    /// </summary>
    /// <param name="context">Контекст выполняемого действия.</param>
    /// <param name="next">Вызов следующего фильтра или действия.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
        {
            await next();
            return;
        }

        await using var transaction = await database.Database.BeginTransactionAsync(
            context.HttpContext.RequestAborted);
        var result = await next();
        var status = result.Result switch
        {
            ObjectResult value => value.StatusCode ?? StatusCodes.Status200OK,
            StatusCodeResult value => value.StatusCode,
            _ => StatusCodes.Status200OK
        };

        if (result.Exception is null && status < StatusCodes.Status400BadRequest)
        {
            await database.SaveChangesAsync(context.HttpContext.RequestAborted);
            await transaction.CommitAsync(context.HttpContext.RequestAborted);
        }
    }
}
