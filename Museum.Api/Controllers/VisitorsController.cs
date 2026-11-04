using Microsoft.AspNetCore.Mvc;
using Museum.Application.Repositories;
using Museum.Contracts.Visitors;
using Museum.Domain.Entities;

namespace Museum.Api.Controllers;

/// <summary>Управление посетителями музея.</summary>
[ApiController]
[Route("api/[controller]")]
public class VisitorsController(IRepository<Visitor> visitors, IRepository<Ticket> tickets, ILogger<VisitorsController> logger) : ControllerBase
{
    /// <summary>Возвращает список всех посетителей.</summary>
    [HttpGet]
    public ActionResult<IReadOnlyList<VisitorDto>> GetAll()
    {
        logger.LogInformation("Получение списка посетителей.");
        return Ok(visitors.GetAll().Select(Map).ToList());
    }

    /// <summary>Возвращает посетителя по идентификатору.</summary>
    [HttpGet("{id:int}")]
    public ActionResult<VisitorDto> GetById(int id)
    {
        logger.LogInformation("Получение посетителя {Id}.", id);
        var visitor = visitors.GetById(id);
        return visitor is null ? NotFound() : Ok(Map(visitor));
    }

    /// <summary>Создаёт нового посетителя.</summary>
    [HttpPost]
    public ActionResult<VisitorDto> Create(VisitorCreateUpdateDto dto)
    {
        logger.LogInformation("Создание посетителя.");
        var visitor = visitors.Create(new Visitor { FullName = dto.FullName, Phone = dto.Phone, BirthDate = dto.BirthDate });
        return CreatedAtAction(nameof(GetById), new { id = visitor.Id }, Map(visitor));
    }

    /// <summary>Обновляет данные посетителя.</summary>
    [HttpPut("{id:int}")]
    public ActionResult Update(int id, VisitorCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление посетителя {Id}.", id);
        var visitor = visitors.GetById(id);
        if (visitor is null) return NotFound();
        visitor.FullName = dto.FullName;
        visitor.Phone = dto.Phone;
        visitor.BirthDate = dto.BirthDate;
        visitors.Update(visitor);
        return NoContent();
    }

    /// <summary>Удаляет посетителя без билетов.</summary>
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление посетителя {Id}.", id);
        if (visitors.GetById(id) is null) return NotFound();
        if (tickets.GetAll().Any(ticket => ticket.VisitorId == id))
            return Conflict(new { Message = "Нельзя удалить посетителя, у которого есть билеты." });
        visitors.Delete(id);
        return NoContent();
    }

    private static VisitorDto Map(Visitor visitor) => new(visitor.Id, visitor.FullName, visitor.Phone, visitor.BirthDate);
}
