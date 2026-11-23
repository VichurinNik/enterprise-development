using Museum.Contracts.Tickets;

namespace Museum.Application.Services;

/// <summary>
/// Сервис управления билетами на экскурсии.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Возвращает список всех билетов.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список данных билетов.</returns>
    public Task<IReadOnlyList<TicketDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает данные билета по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные билета или null, если запись не найдена.</returns>
    public Task<TicketDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись билета.
    /// </summary>
    /// <param name="dto">Данные для создания билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    public Task<TicketDto> CreateAsync(TicketCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные билета.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="dto">Новые данные билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    public Task<bool> UpdateAsync(int id, TicketCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись билета.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
