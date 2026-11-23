using Museum.Contracts.Exhibitions;

namespace Museum.Application.Services;

/// <summary>
/// Сервис управления музейными выставками.
/// </summary>
public interface IExhibitionService
{
    /// <summary>
    /// Возвращает список всех выставок.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список данных выставок.</returns>
    public Task<IReadOnlyList<ExhibitionDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает данные выставки по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные выставки или null, если запись не найдена.</returns>
    public Task<ExhibitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись выставки.
    /// </summary>
    /// <param name="dto">Данные для создания выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    public Task<ExhibitionDto> CreateAsync(ExhibitionCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="dto">Новые данные выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    public Task<bool> UpdateAsync(int id, ExhibitionCreateUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
