using Museum.Application.Exceptions;
using Museum.Domain.Repositories;
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
    public async Task<IReadOnlyList<VisitorDto>> GetAllAsync(CancellationToken cancellationToken) => (await visitors.GetAllAsync(cancellationToken)).Select(MapToDto).ToList();

    /// <inheritdoc />
    public async Task<VisitorDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var visitor = await visitors.GetByIdAsync(id, cancellationToken);
        return visitor is null ? null : MapToDto(visitor);
    }

    /// <inheritdoc />
    public async Task<VisitorDto> CreateAsync(VisitorCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var visitor = await visitors.CreateAsync(new Visitor
        {
            FullName = dto.FullName,
            Phone = dto.Phone,
            BirthDate = dto.BirthDate
        }, cancellationToken);
        return MapToDto(visitor);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(int id, VisitorCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var visitor = await visitors.GetByIdAsync(id, cancellationToken);
        if (visitor is null)
        {
            return false;
        }

        visitor.FullName = dto.FullName;
        visitor.Phone = dto.Phone;
        visitor.BirthDate = dto.BirthDate;
        return await visitors.UpdateAsync(visitor, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await visitors.GetByIdAsync(id, cancellationToken) is null)
        {
            return false;
        }

        if ((await tickets.GetAllAsync(cancellationToken)).Any(ticket => ticket.VisitorId == id))
        {
            throw new EntityInUseException("Нельзя удалить посетителя, у которого есть билеты.");
        }

        await visitors.DeleteAsync(id, cancellationToken);
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
