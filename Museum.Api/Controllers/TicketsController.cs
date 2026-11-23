using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Tickets;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер управления билетами на экскурсии.
/// </summary>
/// <param name="service">Сервис управления билетами на экскурсии.</param>
/// <param name="logger">Журнал запросов контроллера.</param>
[ApiController]
[Route("api/[controller]")]
public class TicketsController(ITicketService service, ILogger<TicketsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех билетов.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список билетов.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Получение списка билетов.");
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    /// <summary>
    /// Возвращает данные билета по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные билета или ответ об отсутствии записи.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDto>> GetById(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Получение билета {Id}.", id);
        var item = await service.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Создаёт запись билета.
    /// </summary>
    /// <param name="dto">Данные для создания билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданная запись и адрес её получения.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketDto>> Create([FromBody] TicketCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Создание билета.");
        var created = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные билета.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="dto">Новые данные билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ без содержимого или сообщение об отсутствии записи.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Update(int id, [FromBody] TicketCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Обновление билета {Id}.", id);
        return await service.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Удаляет запись билета.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ без содержимого либо ошибка отсутствия записи.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Удаление билета {Id}.", id);
        return await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
