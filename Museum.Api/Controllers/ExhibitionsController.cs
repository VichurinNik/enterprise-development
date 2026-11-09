using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Exhibitions;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер управления музейными выставками.
/// </summary>
/// <param name="service">Сервис управления музейными выставками.</param>
/// <param name="logger">Журнал запросов контроллера.</param>
[ApiController]
[Route("api/[controller]")]
public class ExhibitionsController(IExhibitionService service, ILogger<ExhibitionsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех выставок.
    /// </summary>
    /// <returns>Список выставок.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExhibitionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<IReadOnlyList<ExhibitionDto>> GetAll()
    {
        logger.LogInformation("Получение списка выставок.");
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные выставки по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <returns>Данные выставки или ответ об отсутствии записи.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<ExhibitionDto> GetById(int id)
    {
        logger.LogInformation("Получение выставки {Id}.", id);
        var item = service.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Создаёт запись выставки.
    /// </summary>
    /// <param name="dto">Данные для создания выставки.</param>
    /// <returns>Созданная запись и адрес её получения.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<ExhibitionDto> Create([FromBody] ExhibitionCreateUpdateDto dto)
    {
        logger.LogInformation("Создание выставки.");
        var created = service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="dto">Новые данные выставки.</param>
    /// <returns>Ответ без содержимого или сообщение об отсутствии записи.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult Update(int id, [FromBody] ExhibitionCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление выставки {Id}.", id);
        return service.Update(id, dto) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Удаляет запись выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <returns>Ответ без содержимого либо ошибка отсутствия или использования записи.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление выставки {Id}.", id);
        return service.Delete(id) ? NoContent() : NotFound();
    }
}
