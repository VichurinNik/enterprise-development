using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Application.Repositories;
using Museum.Contracts.Exhibitions;
using Museum.Domain.Data;
using Museum.Domain.Entities;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер для управления музейными выставками.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExhibitionsController(
    IExhibitionService exhibitionService,
    IRepository<Exhibition> exhibitions,
    MuseumContext context,
    ILogger<ExhibitionsController> logger) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех выставок.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExhibitionDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ExhibitionDto>> GetAll()
    {
        logger.LogInformation("Запрошен список выставок.");
        var exhibitions = exhibitionService.GetAll();
        return Ok(exhibitions);
    }

    /// <summary>
    /// Возвращает выставку по её уникальному идентификатору.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ExhibitionDto> GetById(int id)
    {
        logger.LogInformation("Запрошена выставка {Id}.", id);
        var exhibition = exhibitionService.GetById(id);
        if (exhibition is null)
        {
            return NotFound(new { Message = $"Выставка с Id = {id} не найдена." });
        }

        return Ok(exhibition);
    }

    /// <summary>
    /// Создаёт новую выставку.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status201Created)]
    public ActionResult<ExhibitionDto> Create([FromBody] ExhibitionCreateUpdateDto dto)
    {
        logger.LogInformation("Создание выставки.");
        var created = exhibitionService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные существующей выставки.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Update(int id, [FromBody] ExhibitionCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление выставки {Id}.", id);
        var updated = exhibitionService.Update(id, dto);
        if (!updated)
        {
            return NotFound(new { Message = $"Не удалось обновить: выставка с Id = {id} не найдена." });
        }

        return NoContent();
    }

    /// <summary>
    /// Удаляет выставку по идентификатору.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление выставки {Id}.", id);
        if (exhibitions.GetById(id) is null) return NotFound();
        if (context.ExcursionExhibitions.Any(link => link.ExhibitionId == id))
            return Conflict(new { Message = "Нельзя удалить выставку, которая входит в экскурсию." });
        exhibitionService.Delete(id);
        return NoContent();
    }
}
