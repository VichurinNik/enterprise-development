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
    /// <returns>Список данных экскурсий.</returns>
    IReadOnlyList<ExcursionsDto> GetAll();

    /// <summary>
    /// Возвращает данные экскурсии по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <returns>Данные экскурсии или null, если запись не найдена.</returns>
    ExcursionsDto? GetById(int id);

    /// <summary>
    /// Создаёт запись экскурсии.
    /// </summary>
    /// <param name="dto">Данные для создания экскурсии.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    ExcursionsDto Create(ExcursionCreateUpdateDto dto);

    /// <summary>
    /// Обновляет данные экскурсии.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <param name="dto">Новые данные экскурсии.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    bool Update(int id, ExcursionCreateUpdateDto dto);

    /// <summary>
    /// Удаляет запись экскурсии.
    /// </summary>
    /// <param name="id">Идентификатор экскурсии.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    bool Delete(int id);
}
