using Museum.Contracts.Excursions;

namespace Museum.Application.Services;

/// <summary>
/// Сервис управления музейными экскурсиями.
/// </summary>
public interface IExcursionService
{
    /// <summary>
    /// Возвращает список всех экскурсий.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список данных экскурсий.</returns>
    public Task<IReadOnlyList<ExcursionsDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает данные экскурсии по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные экскурсии или null, если запись не найдена.</returns>
    public Task<ExcursionsDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись экскурсии.
    /// </summary>
    /// <param name="dto">Данные для создания экскурсии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    public Task<ExcursionsDto> CreateAsync(ExcursionCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные экскурсии.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <param name="dto">Новые данные экскурсии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    public Task<bool> UpdateAsync(int id, ExcursionCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись экскурсии.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
