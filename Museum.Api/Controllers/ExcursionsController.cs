using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Excursions;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер управления музейными экскурсиями.
/// </summary>
/// <param name="service">Сервис управления музейными экскурсиями.</param>
/// <param name="logger">Журнал запросов контроллера.</param>
[ApiController]
[Route("api/[controller]")]
public class ExcursionsController(IExcursionService service, ILogger<ExcursionsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех экскурсий.
    /// </summary>
    /// <returns>Список экскурсий.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExcursionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<ExcursionsDto>> GetAll()
    {
        logger.LogInformation("Получение списка экскурсий.");
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные экскурсии по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <returns>Данные экскурсии или ответ об отсутствии записи.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExcursionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<ExcursionsDto> GetById(int id)
    {
        logger.LogInformation("Получение экскурсии {Id}.", id);
        var item = service.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Создаёт запись экскурсии.
    /// </summary>
    /// <param name="dto">Данные для создания экскурсии.</param>
    /// <returns>Созданная запись и адрес её получения.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExcursionsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<ExcursionsDto> Create([FromBody] ExcursionCreateUpdateDto dto)
    {
        logger.LogInformation("Создание экскурсии.");
        var created = service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные экскурсии.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <param name="dto">Новые данные экскурсии.</param>
    /// <returns>Ответ без содержимого или сообщение об отсутствии записи.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult Update(int id, [FromBody] ExcursionCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление экскурсии {Id}.", id);
        return service.Update(id, dto) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Удаляет запись экскурсии.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <returns>Ответ без содержимого либо ошибка отсутствия или использования записи.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление экскурсии {Id}.", id);
        return service.Delete(id) ? NoContent() : NotFound();
    }
}
