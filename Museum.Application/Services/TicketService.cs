using Museum.Domain.Repositories;
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
    public async Task<IReadOnlyList<TicketDto>> GetAllAsync(CancellationToken cancellationToken) => (await tickets.GetAllAsync(cancellationToken)).Select(MapToDto).ToList();

    /// <inheritdoc />
    public async Task<TicketDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await tickets.GetByIdAsync(id, cancellationToken);
        return ticket is null ? null : MapToDto(ticket);
    }

    /// <inheritdoc />
    public async Task<TicketDto> CreateAsync(TicketCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var (excursion, visitor) = await ResolveRelationsAsync(dto, cancellationToken);
        var ticket = await tickets.CreateAsync(new Ticket
        {
            ExcursionId = excursion.Id,
            Excursion = excursion,
            VisitorId = visitor.Id,
            Visitor = visitor,
            TicketType = dto.TicketType,
            Price = dto.Price
        }, cancellationToken);
        if (!excursion.Tickets.Contains(ticket))
            excursion.Tickets.Add(ticket);
        if (!visitor.Tickets.Contains(ticket))
            visitor.Tickets.Add(ticket);
        return MapToDto(ticket);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(int id, TicketCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var ticket = await tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return false;
        }

        var (excursion, visitor) = await ResolveRelationsAsync(dto, cancellationToken);
        ticket.Excursion.Tickets.Remove(ticket);
        ticket.Visitor.Tickets.Remove(ticket);
        ticket.ExcursionId = excursion.Id;
        ticket.Excursion = excursion;
        ticket.VisitorId = visitor.Id;
        ticket.Visitor = visitor;
        ticket.TicketType = dto.TicketType;
        ticket.Price = dto.Price;
        if (!excursion.Tickets.Contains(ticket))
            excursion.Tickets.Add(ticket);
        if (!visitor.Tickets.Contains(ticket))
            visitor.Tickets.Add(ticket);
        return await tickets.UpdateAsync(ticket, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return false;
        }

        ticket.Excursion.Tickets.Remove(ticket);
        ticket.Visitor.Tickets.Remove(ticket);
        await tickets.DeleteAsync(id, cancellationToken);
        return true;
    }

    /// <summary>
    /// Проверяет стоимость билета и находит связанные записи до изменения данных.
    /// </summary>
    /// <param name="dto">Данные создаваемого или обновляемого билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Найденные экскурсия и посетитель.</returns>
    private async Task<(Excursion Excursion, Visitor Visitor)> ResolveRelationsAsync(
        TicketCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        if (dto.Price < 0)
        {
            throw new ArgumentException("Стоимость не может быть отрицательной.", nameof(dto));
        }

        var excursion = await excursions.GetByIdAsync(dto.ExcursionId, cancellationToken)
            ?? throw new ArgumentException("Экскурсия не найдена.", nameof(dto));
        var visitor = await visitors.GetByIdAsync(dto.VisitorId, cancellationToken)
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
