using Microsoft.Extensions.Logging;
using Museum.Application.Repositories;
using Museum.Contracts.Exhibitions;
using Museum.Domain.Entities;

namespace Museum.Application.Services;

/// <summary>
/// Реализация службы управления выставками с поддержкой логирования.
/// </summary>
public class ExhibitionService(
    IRepository<Exhibition> repository,
    ILogger<ExhibitionService> logger) : IExhibitionService
{
    /// <inheritdoc />
    public IReadOnlyList<ExhibitionDto> GetAll()
    {
        logger.LogInformation("Запрос на получение всех выставок.");
        return repository.GetAll().Select(MapToDto).ToList();
    }

    /// <inheritdoc />
    public ExhibitionDto? GetById(int id)
    {
        logger.LogInformation("Запрос на получение выставки с Id = {Id}.", id);
        var entity = repository.GetById(id);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public ExhibitionDto Create(ExhibitionCreateUpdateDto dto)
    {
        var entity = new Exhibition
        {
            Name = dto.Name,
            Theme = dto.Theme,
            HallNumber = dto.HallNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        var created = repository.Create(entity);
        logger.LogInformation("Создана новая выставка с Id = {Id}.", created.Id);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public bool Update(int id, ExhibitionCreateUpdateDto dto)
    {
        var existing = repository.GetById(id);
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

        var result = repository.Update(existing);
        if (result)
        {
            logger.LogInformation("Обновлена выставка с Id = {Id}.", id);
        }

        return result;
    }

    /// <inheritdoc />
    public void Delete(int id)
    {
        repository.Delete(id);
        logger.LogInformation("Удалена выставка с Id = {Id}.", id);
    }

    private static ExhibitionDto MapToDto(Exhibition entity) =>
        new(entity.Id, entity.Name, entity.Theme, entity.HallNumber, entity.StartDate, entity.EndDate);
}