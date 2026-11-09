using Museum.Application.Exceptions;
using Museum.Application.Repositories;
using Museum.Contracts.Excursions;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Управляет экскурсиями и составом посещаемых выставок.
/// </summary>
/// <param name="excursions">Репозиторий экскурсий.</param>
/// <param name="exhibitions">Репозиторий выставок.</param>
/// <param name="tickets">Репозиторий билетов.</param>
/// <param name="links">Репозиторий связей экскурсий с выставками.</param>
public class ExcursionService(
    IRepository<Excursion> excursions,
    IRepository<Exhibition> exhibitions,
    IRepository<Ticket> tickets,
    IExcursionExhibitionRepository links) : IExcursionService
{
    /// <inheritdoc />
    public IReadOnlyList<ExcursionsDto> GetAll() => excursions.GetAll().Select(MapToDto).ToList();

    /// <inheritdoc />
    public ExcursionsDto? GetById(int id)
    {
        var excursion = excursions.GetById(id);
        return excursion is null ? null : MapToDto(excursion);
    }

    /// <inheritdoc />
    public ExcursionsDto Create(ExcursionCreateUpdateDto dto)
    {
        var selected = ResolveExhibitions(dto);
        var excursion = excursions.Create(new Excursion
        {
            Date = dto.Date,
            Duration = dto.Duration
        });
        links.ReplaceForExcursion(excursion, selected);
        return MapToDto(excursion);
    }

    /// <inheritdoc />
    public bool Update(int id, ExcursionCreateUpdateDto dto)
    {
        var excursion = excursions.GetById(id);
        if (excursion is null)
        {
            return false;
        }

        var selected = ResolveExhibitions(dto);
        excursion.Date = dto.Date;
        excursion.Duration = dto.Duration;
        links.ReplaceForExcursion(excursion, selected);
        return excursions.Update(excursion);
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        var excursion = excursions.GetById(id);
        if (excursion is null)
        {
            return false;
        }

        if (tickets.GetAll().Any(ticket => ticket.ExcursionId == id))
        {
            throw new EntityInUseException("Нельзя удалить экскурсию, на которую проданы билеты.");
        }

        links.RemoveForExcursion(excursion);
        excursions.Delete(id);
        return true;
    }

    /// <summary>
    /// Проверяет продолжительность экскурсии и находит выставки до изменения данных.
    /// </summary>
    /// <param name="dto">Данные создаваемой или обновляемой экскурсии.</param>
    /// <returns>Список найденных выставок без повторений.</returns>
    private IReadOnlyList<Exhibition> ResolveExhibitions(ExcursionCreateUpdateDto dto)
    {
        if (dto.Duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Продолжительность должна быть положительной.", nameof(dto));
        }

        if (dto.ExhibitionIds is null || dto.ExhibitionIds.Count != dto.ExhibitionIds.Distinct().Count())
        {
            throw new ArgumentException("Список выставок не задан или содержит повторения.", nameof(dto));
        }

        return dto.ExhibitionIds
            .Select(id => exhibitions.GetById(id)
                ?? throw new ArgumentException($"Выставка с идентификатором {id} не найдена.", nameof(dto)))
            .ToList();
    }

    /// <summary>
    /// Преобразует экскурсию в контракт ответа.
    /// </summary>
    /// <param name="excursion">Экскурсия из хранилища.</param>
    /// <returns>Данные экскурсии для передачи клиенту.</returns>
    private static ExcursionsDto MapToDto(Excursion excursion) =>
        new(excursion.Id, excursion.Date, excursion.Duration,
            excursion.Exhibitions.Select(link => link.ExhibitionId).ToList());
}
