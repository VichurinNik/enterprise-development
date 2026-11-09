using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Visitors;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер управления посетителями музея.
/// </summary>
/// <param name="service">Сервис управления посетителями музея.</param>
/// <param name="logger">Журнал запросов контроллера.</param>
[ApiController]
[Route("api/[controller]")]
public class VisitorsController(IVisitorService service, ILogger<VisitorsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех посетителей.
    /// </summary>
    /// <returns>Список посетителей.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VisitorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<VisitorDto>> GetAll()
    {
        logger.LogInformation("Получение списка посетителей.");
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные посетителя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <returns>Данные посетителя или ответ об отсутствии записи.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VisitorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<VisitorDto> GetById(int id)
    {
        logger.LogInformation("Получение посетителя {Id}.", id);
        var item = service.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Создаёт запись посетителя.
    /// </summary>
    /// <param name="dto">Данные для создания посетителя.</param>
    /// <returns>Созданная запись и адрес её получения.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(VisitorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<VisitorDto> Create([FromBody] VisitorCreateUpdateDto dto)
    {
        logger.LogInformation("Создание посетителя.");
        var created = service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные посетителя.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <param name="dto">Новые данные посетителя.</param>
    /// <returns>Ответ без содержимого или сообщение об отсутствии записи.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult Update(int id, [FromBody] VisitorCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление посетителя {Id}.", id);
        return service.Update(id, dto) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Удаляет запись посетителя.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <returns>Ответ без содержимого либо ошибка отсутствия или использования записи.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление посетителя {Id}.", id);
        return service.Delete(id) ? NoContent() : NotFound();
    }
}
