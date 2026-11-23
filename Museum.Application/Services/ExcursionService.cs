using Museum.Application.Exceptions;
using Museum.Domain.Repositories;
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
    public async Task<IReadOnlyList<ExcursionsDto>> GetAllAsync(CancellationToken cancellationToken) => (await excursions.GetAllAsync(cancellationToken)).Select(MapToDto).ToList();

    /// <inheritdoc />
    public async Task<ExcursionsDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var excursion = await excursions.GetByIdAsync(id, cancellationToken);
        return excursion is null ? null : MapToDto(excursion);
    }

    /// <inheritdoc />
    public async Task<ExcursionsDto> CreateAsync(ExcursionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var selected = await ResolveExhibitionsAsync(dto, cancellationToken);
        var excursion = await excursions.CreateAsync(new Excursion
        {
            Date = dto.Date,
            Duration = dto.Duration
        }, cancellationToken);
        await links.ReplaceForExcursionAsync(excursion, selected, cancellationToken);
        return MapToDto(excursion);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(int id, ExcursionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var excursion = await excursions.GetByIdAsync(id, cancellationToken);
        if (excursion is null)
        {
            return false;
        }

        var selected = await ResolveExhibitionsAsync(dto, cancellationToken);
        excursion.Date = dto.Date;
        excursion.Duration = dto.Duration;
        await links.ReplaceForExcursionAsync(excursion, selected, cancellationToken);
        return await excursions.UpdateAsync(excursion, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var excursion = await excursions.GetByIdAsync(id, cancellationToken);
        if (excursion is null)
        {
            return false;
        }

        if ((await tickets.GetAllAsync(cancellationToken)).Any(ticket => ticket.ExcursionId == id))
        {
            throw new EntityInUseException("Нельзя удалить экскурсию, на которую проданы билеты.");
        }

        await links.RemoveForExcursionAsync(excursion, cancellationToken);
        await excursions.DeleteAsync(id, cancellationToken);
        return true;
    }

    /// <summary>
    /// Проверяет продолжительность экскурсии и находит выставки до изменения данных.
    /// </summary>
    /// <param name="dto">Данные создаваемой или обновляемой экскурсии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список найденных выставок без повторений.</returns>
    private async Task<IReadOnlyList<Exhibition>> ResolveExhibitionsAsync(
        ExcursionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        if (dto.Duration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Продолжительность должна быть положительной.", nameof(dto));
        }

        if (dto.ExhibitionIds is null || dto.ExhibitionIds.Count != dto.ExhibitionIds.Distinct().Count())
        {
            throw new ArgumentException("Список выставок не задан или содержит повторения.", nameof(dto));
        }

        var selected = new List<Exhibition>(dto.ExhibitionIds.Count);
        foreach (var id in dto.ExhibitionIds)
        {
            var exhibition = await exhibitions.GetByIdAsync(id, cancellationToken)
                ?? throw new ArgumentException($"Выставка с идентификатором {id} не найдена.", nameof(dto));
            selected.Add(exhibition);
        }

        return selected;
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
