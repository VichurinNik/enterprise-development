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
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список выставок.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExhibitionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ExhibitionDto>>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Получение списка выставок.");
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    /// <summary>
    /// Возвращает данные выставки по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные выставки или ответ об отсутствии записи.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExhibitionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Получение выставки {Id}.", id);
        var item = await service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Создаёт запись выставки.
    /// </summary>
    /// <param name="dto">Данные для создания выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданная запись и адрес её получения.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExhibitionDto>> Create([FromBody] ExhibitionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Создание выставки.");
        var created = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="dto">Новые данные выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ без содержимого или сообщение об отсутствии записи.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(int id, [FromBody] ExhibitionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Обновление выставки {Id}.", id);
        return await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Удаляет запись выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ без содержимого либо ошибка отсутствия или использования записи.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Удаление выставки {Id}.", id);
        return await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
