using Microsoft.Extensions.Logging;
using Museum.Application.Exceptions;
using Museum.Domain.Repositories;
using Museum.Contracts.Exhibitions;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Реализация службы управления выставками с поддержкой логирования.
/// </summary>
/// <param name="repository">Репозиторий выставок.</param>
/// <param name="links">Репозиторий связей с экскурсиями.</param>
/// <param name="logger">Журнал операций сервиса.</param>
public class ExhibitionService(
    IRepository<Exhibition> repository,
    IExcursionExhibitionRepository links,
    ILogger<ExhibitionService> logger) : IExhibitionService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<ExhibitionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение всех выставок.");
        return (await repository.GetAllAsync(cancellationToken)).Select(MapToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<ExhibitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение выставки с Id = {Id}.", id);
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<ExhibitionDto> CreateAsync(ExhibitionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = new Exhibition
        {
            Name = dto.Name,
            Theme = dto.Theme,
            HallNumber = dto.HallNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        var created = await repository.CreateAsync(entity, cancellationToken);
        logger.LogInformation("Создана новая выставка с Id = {Id}.", created.Id);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(int id, ExhibitionCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            logger.LogWarning("Не удалось обновить: выставка с Id = {Id} не найдена.", id);
            return false;
        }

        existing.Name = dto.Name;
        existing.Theme = dto.Theme;
        existing.HallNumber = dto.HallNumber;
        existing.StartDate = dto.StartDate;
        existing.EndDate = dto.EndDate;

        var result = await repository.UpdateAsync(existing, cancellationToken);
        if (result)
        {
            logger.LogInformation("Обновлена выставка с Id = {Id}.", id);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is null)
        {
            return false;
        }

        if (await links.ExistsForExhibitionAsync(id, cancellationToken))
        {
            throw new EntityInUseException("Нельзя удалить выставку, которая входит в экскурсию.");
        }

        await repository.DeleteAsync(id, cancellationToken);
        logger.LogInformation("Удалена выставка с Id = {Id}.", id);
        return true;
    }

    /// <summary>
    /// Преобразует выставку в контракт ответа.
    /// </summary>
    /// <param name="entity">Выставка из хранилища.</param>
    /// <returns>Данные выставки для передачи клиенту.</returns>
    private static ExhibitionDto MapToDto(Exhibition entity) =>
        new(entity.Id, entity.Name, entity.Theme, entity.HallNumber, entity.StartDate, entity.EndDate);
}
