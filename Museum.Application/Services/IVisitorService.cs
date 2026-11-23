using Museum.Contracts.Visitors;

namespace Museum.Application.Services;

/// <summary>
/// Сервис управления посетителями музея.
/// </summary>
public interface IVisitorService
{
    /// <summary>
    /// Возвращает список всех посетителей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список данных посетителей.</returns>
    public Task<IReadOnlyList<VisitorDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает данные посетителя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные посетителя или null, если запись не найдена.</returns>
    public Task<VisitorDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись посетителя.
    /// </summary>
    /// <param name="dto">Данные для создания посетителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    public Task<VisitorDto> CreateAsync(VisitorCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные посетителя.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <param name="dto">Новые данные посетителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    public Task<bool> UpdateAsync(int id, VisitorCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись посетителя.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
