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
    /// <returns>Список данных выставок.</returns>
    IReadOnlyList<ExhibitionDto> GetAll();

    /// <summary>
    /// Возвращает данные выставки по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <returns>Данные выставки или null, если запись не найдена.</returns>
    ExhibitionDto? GetById(int id);

    /// <summary>
    /// Создаёт запись выставки.
    /// </summary>
    /// <param name="dto">Данные для создания выставки.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    ExhibitionDto Create(ExhibitionCreateUpdateDto dto);

    /// <summary>
    /// Обновляет данные выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <param name="dto">Новые данные выставки.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    bool Update(int id, ExhibitionCreateUpdateDto dto);

    /// <summary>
    /// Удаляет запись выставки.
    /// </summary>
    /// <param name="id">Идентификатор выставки.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    bool Delete(int id);
}
