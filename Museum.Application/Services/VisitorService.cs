using Museum.Application.Exceptions;
using Museum.Application.Repositories;
using Museum.Contracts.Visitors;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Управляет данными посетителей и проверяет связи с билетами.
/// </summary>
/// <param name="visitors">Репозиторий посетителей.</param>
/// <param name="tickets">Репозиторий билетов.</param>
public class VisitorService(IRepository<Visitor> visitors, IRepository<Ticket> tickets) : IVisitorService
{
    /// <inheritdoc />
    public IReadOnlyList<VisitorDto> GetAll() => visitors.GetAll().Select(MapToDto).ToList();

    /// <inheritdoc />
    public VisitorDto? GetById(int id)
    {
        var visitor = visitors.GetById(id);
        return visitor is null ? null : MapToDto(visitor);
    }

    /// <inheritdoc />
    public VisitorDto Create(VisitorCreateUpdateDto dto)
    {
        var visitor = visitors.Create(new Visitor
        {
            FullName = dto.FullName,
            Phone = dto.Phone,
            BirthDate = dto.BirthDate
        });
        return MapToDto(visitor);
    }

    /// <inheritdoc />
    public bool Update(int id, VisitorCreateUpdateDto dto)
    {
        var visitor = visitors.GetById(id);
        if (visitor is null)
        {
            return false;
        }

        visitor.FullName = dto.FullName;
        visitor.Phone = dto.Phone;
        visitor.BirthDate = dto.BirthDate;
        return visitors.Update(visitor);
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        if (visitors.GetById(id) is null)
        {
            return false;
        }

        if (tickets.GetAll().Any(ticket => ticket.VisitorId == id))
        {
            throw new EntityInUseException("Нельзя удалить посетителя, у которого есть билеты.");
        }

        visitors.Delete(id);
        return true;
    }

    /// <summary>
    /// Преобразует посетителя в контракт ответа.
    /// </summary>
    /// <param name="visitor">Посетитель из хранилища.</param>
    /// <returns>Данные посетителя для передачи клиенту.</returns>
    private static VisitorDto MapToDto(Visitor visitor) =>
        new(visitor.Id, visitor.FullName, visitor.Phone, visitor.BirthDate);
}
