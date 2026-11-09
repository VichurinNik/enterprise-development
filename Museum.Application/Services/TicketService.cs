using Museum.Application.Repositories;
using Museum.Contracts.Tickets;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Управляет билетами и их связями с экскурсиями и посетителями.
/// </summary>
/// <param name="tickets">Репозиторий билетов.</param>
/// <param name="excursions">Репозиторий экскурсий.</param>
/// <param name="visitors">Репозиторий посетителей.</param>
public class TicketService(
    IRepository<Ticket> tickets,
    IRepository<Excursion> excursions,
    IRepository<Visitor> visitors) : ITicketService
{
    /// <inheritdoc />
    public IReadOnlyList<TicketDto> GetAll() => tickets.GetAll().Select(MapToDto).ToList();

    /// <inheritdoc />
    public TicketDto? GetById(int id)
    {
        var ticket = tickets.GetById(id);
        return ticket is null ? null : MapToDto(ticket);
    }

    /// <inheritdoc />
    public TicketDto Create(TicketCreateUpdateDto dto)
    {
        var (excursion, visitor) = ResolveRelations(dto);
        var ticket = tickets.Create(new Ticket
        {
            ExcursionId = excursion.Id,
            Excursion = excursion,
            VisitorId = visitor.Id,
            Visitor = visitor,
            TicketType = dto.TicketType,
            Price = dto.Price
        });
        excursion.Tickets.Add(ticket);
        visitor.Tickets.Add(ticket);
        return MapToDto(ticket);
    }

    /// <inheritdoc />
    public bool Update(int id, TicketCreateUpdateDto dto)
    {
        var ticket = tickets.GetById(id);
        if (ticket is null)
        {
            return false;
        }

        var (excursion, visitor) = ResolveRelations(dto);
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
        return tickets.Update(ticket);
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        var ticket = tickets.GetById(id);
        if (ticket is null)
        {
            return false;
        }

        ticket.Excursion.Tickets.Remove(ticket);
        ticket.Visitor.Tickets.Remove(ticket);
        tickets.Delete(id);
        return true;
    }

    /// <summary>
    /// Проверяет стоимость билета и находит связанные записи до изменения данных.
    /// </summary>
    /// <param name="dto">Данные создаваемого или обновляемого билета.</param>
    /// <returns>Найденные экскурсия и посетитель.</returns>
    private (Excursion Excursion, Visitor Visitor) ResolveRelations(TicketCreateUpdateDto dto)
    {
        if (dto.Price < 0)
        {
            throw new ArgumentException("Стоимость не может быть отрицательной.", nameof(dto));
        }

        var excursion = excursions.GetById(dto.ExcursionId)
            ?? throw new ArgumentException("Экскурсия не найдена.", nameof(dto));
        var visitor = visitors.GetById(dto.VisitorId)
            ?? throw new ArgumentException("Посетитель не найден.", nameof(dto));
        return (excursion, visitor);
    }

    /// <summary>
    /// Преобразует билет в контракт ответа.
    /// </summary>
    /// <param name="ticket">Билет из хранилища.</param>
    /// <returns>Данные билета для передачи клиенту.</returns>
    private static TicketDto MapToDto(Ticket ticket) =>
        new(ticket.Id, ticket.ExcursionId, ticket.VisitorId, ticket.TicketType, ticket.Price);
}
