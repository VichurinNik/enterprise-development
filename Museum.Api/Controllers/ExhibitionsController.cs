using Microsoft.AspNetCore.Mvc;
using Museum.Application.Services;
using Museum.Contracts.Exhibitions;

namespace Museum.Api.Controllers;

/// <summary>
/// Контроллер для управления музейными выставками.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExhibitionsController(IExhibitionService exhibitionService) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех выставок.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExhibitionDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var exhibitions = exhibitionService.GetAll();
        return Ok(exhibitions);
    }

    /// <summary>
    /// Возвращает выставку по её уникальному идентификатору.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
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
    public IActionResult Create([FromBody] ExhibitionCreateUpdateDto dto)
    {
        var created = exhibitionService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновляет данные существующей выставки.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] ExhibitionCreateUpdateDto dto)
    {
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
    public IActionResult Delete(int id)
    {
        exhibitionService.Delete(id);
        return NoContent();
    }
}