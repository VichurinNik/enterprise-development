using Microsoft.AspNetCore.Mvc;
using Museum.Application.Repositories;
using Museum.Contracts.Tickets;
using Museum.Domain.Entities;

namespace Museum.Api.Controllers;

/// <summary>Управление билетами на экскурсии.</summary>
[ApiController]
[Route("api/[controller]")]
public class TicketsController(
    IRepository<Ticket> tickets,
    IRepository<Excursion> excursions,
    IRepository<Visitor> visitors,
    ILogger<TicketsController> logger) : ControllerBase
{
    /// <summary>Возвращает список всех билетов.</summary>
    [HttpGet]
    public ActionResult<IReadOnlyList<TicketDto>> GetAll()
    {
        logger.LogInformation("Получение списка билетов.");
        return Ok(tickets.GetAll().Select(Map).ToList());
    }

    /// <summary>Возвращает билет по идентификатору.</summary>
    [HttpGet("{id:int}")]
    public ActionResult<TicketDto> GetById(int id)
    {
        logger.LogInformation("Получение билета {Id}.", id);
        var ticket = tickets.GetById(id);
        return ticket is null ? NotFound() : Ok(Map(ticket));
    }

    /// <summary>Создаёт билет для существующих экскурсии и посетителя.</summary>
    [HttpPost]
    public ActionResult<TicketDto> Create(TicketCreateUpdateDto dto)
    {
        logger.LogInformation("Создание билета для экскурсии {ExcursionId}.", dto.ExcursionId);
        var excursion = excursions.GetById(dto.ExcursionId);
        var visitor = visitors.GetById(dto.VisitorId);
        if (excursion is null || visitor is null)
            return BadRequest(new { Message = "Экскурсия или посетитель не найдены." });
        if (dto.Price < 0) return BadRequest(new { Message = "Стоимость не может быть отрицательной." });
        var ticket = tickets.Create(new Ticket
        {
            ExcursionId = excursion.Id, Excursion = excursion,
            VisitorId = visitor.Id, Visitor = visitor,
            TicketType = dto.TicketType, Price = dto.Price
        });
        excursion.Tickets.Add(ticket);
        visitor.Tickets.Add(ticket);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, Map(ticket));
    }

    /// <summary>Обновляет данные билета.</summary>
    [HttpPut("{id:int}")]
    public ActionResult Update(int id, TicketCreateUpdateDto dto)
    {
        logger.LogInformation("Обновление билета {Id}.", id);
        var ticket = tickets.GetById(id);
        if (ticket is null) return NotFound();
        var excursion = excursions.GetById(dto.ExcursionId);
        var visitor = visitors.GetById(dto.VisitorId);
        if (excursion is null || visitor is null || dto.Price < 0) return BadRequest();
        ticket.Excursion.Tickets.Remove(ticket);
        ticket.Visitor.Tickets.Remove(ticket);
        ticket.ExcursionId = excursion.Id;
        ticket.Excursion = excursion;
        ticket.VisitorId = visitor.Id;
        ticket.Visitor = visitor;
        ticket.TicketType = dto.TicketType;
        ticket.Price = dto.Price;
        excursion.Tickets.Add(ticket);
        visitor.Tickets.Add(ticket);
        tickets.Update(ticket);
        return NoContent();
    }

    /// <summary>Удаляет билет.</summary>
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        logger.LogInformation("Удаление билета {Id}.", id);
        var ticket = tickets.GetById(id);
        if (ticket is null) return NotFound();
        ticket.Excursion.Tickets.Remove(ticket);
        ticket.Visitor.Tickets.Remove(ticket);
        tickets.Delete(id);
        return NoContent();
    }

    private static TicketDto Map(Ticket ticket) =>
        new(ticket.Id, ticket.ExcursionId, ticket.VisitorId, ticket.TicketType, ticket.Price);
}
