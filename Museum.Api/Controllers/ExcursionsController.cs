using Microsoft.AspNetCore.Mvc;
using Museum.Application.Repositories;
using Museum.Contracts.Excursions;
using Museum.Domain.Data;
using Museum.Domain.Entities;

namespace Museum.Api.Controllers;

/// <summary>Управление музейными экскурсиями.</summary>
[ApiController]
[Route("api/[controller]")]
public class ExcursionsController(
    IRepository<Excursion> excursions,
    IRepository<Exhibition> exhibitions,
    IRepository<Ticket> tickets,
    MuseumContext context,
    ILogger<ExcursionsController> logger) : ControllerBase
{
    /// <summary>Возвращает список всех экскурсий.</summary>
    [HttpGet]
    public ActionResult<IReadOnlyList<ExcursionsDto>> GetAll()
    {
        logger.LogInformation("Получение списка экскурсий.");
        return Ok(excursions.GetAll().Select(Map).ToList());
    }

    /// <summary>Возвращает экскурсию по идентификатору.</summary>
    [HttpGet("{id:int}")]
    public ActionResult<ExcursionsDto> GetById(int id)
    {
        logger.LogInformation("Получение экскурсии {Id}.", id);
        var excursion = excursions.GetById(id);
        return excursion is null ? NotFound() : Ok(Map(excursion));
    }

    /// <summary>Создаёт экскурсию и связывает её с выставками.</summary>
    [HttpPost]
    public ActionResult<ExcursionsDto> Create(ExcursionCreateUpdateDto dto)
    {
        logger.LogInformation("Создание экскурсии.");
        var selected = ResolveExhibitions(dto);
        if (selected is null) return BadRequest(new { Message = "Выставка не найдена или список содержит повторения." });
        if (dto.Duration <= TimeSpan.Zero) return BadRequest(new { Message = "Продолжительность должна быть положительной." });
        var excursion = excursions.Create(new Excursion { Date = dto.Date, Duration = dto.Duration });
        AttachExhibitions(excursion, selected);
        return CreatedAtAction(nameof(GetById), new { id = excursion.Id }, Map(excursion));
    }

    /// <summary>Обновляет экскурсию и список её выставок.</summary>
    [HttpPut("{id:int}")]
    public ActionResult Update(int id, ExcursionCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление экскурсии {Id}.", id);
        var excursion = excursions.GetById(id);
        if (excursion is null) return NotFound();
        var selected = ResolveExhibitions(dto);
        if (selected is null || dto.Duration <= TimeSpan.Zero) return BadRequest();
        DetachExhibitions(excursion);
        excursion.Date = dto.Date;
        excursion.Duration = dto.Duration;
        AttachExhibitions(excursion, selected);
        excursions.Update(excursion);
        return NoContent();
    }

    /// <summary>Удаляет экскурсию без проданных билетов.</summary>
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление экскурсии {Id}.", id);
        var excursion = excursions.GetById(id);
        if (excursion is null) return NotFound();
        if (tickets.GetAll().Any(ticket => ticket.ExcursionId == id))
            return Conflict(new { Message = "Нельзя удалить экскурсию, на которую проданы билеты." });
        DetachExhibitions(excursion);
        excursions.Delete(id);
        return NoContent();
    }

    private List<Exhibition>? ResolveExhibitions(ExcursionCreateUpdateDto dto)
    {
        if (dto.ExhibitionIds is null || dto.ExhibitionIds.Count != dto.ExhibitionIds.Distinct().Count())
            return null;
        var selected = dto.ExhibitionIds.Select(exhibitions.GetById).ToList();
        return selected.Any(exhibition => exhibition is null) ? null : selected.Select(exhibition => exhibition!).ToList();
    }

    private void AttachExhibitions(Excursion excursion, IEnumerable<Exhibition> selected)
    {
        foreach (var exhibition in selected)
        {
            var link = new ExcursionExhibition
            {
                ExcursionId = excursion.Id, Excursion = excursion,
                ExhibitionId = exhibition.Id, Exhibition = exhibition
            };
            excursion.Exhibitions.Add(link);
            exhibition.ExcursionExhibitions.Add(link);
            context.ExcursionExhibitions.Add(link);
        }
    }

    private void DetachExhibitions(Excursion excursion)
    {
        foreach (var link in excursion.Exhibitions)
        {
            link.Exhibition.ExcursionExhibitions.Remove(link);
            context.ExcursionExhibitions.Remove(link);
        }
        excursion.Exhibitions.Clear();
    }

    private static ExcursionsDto Map(Excursion excursion) =>
        new(excursion.Id, excursion.Date, excursion.Duration,
            excursion.Exhibitions.Select(link => link.ExhibitionId).ToList());
}
