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
    /// <returns>Список данных посетителей.</returns>
    IReadOnlyList<VisitorDto> GetAll();

    /// <summary>
    /// Возвращает данные посетителя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <returns>Данные посетителя или null, если запись не найдена.</returns>
    VisitorDto? GetById(int id);

    /// <summary>
    /// Создаёт запись посетителя.
    /// </summary>
    /// <param name="dto">Данные для создания посетителя.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    VisitorDto Create(VisitorCreateUpdateDto dto);

    /// <summary>
    /// Обновляет данные посетителя.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <param name="dto">Новые данные посетителя.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    bool Update(int id, VisitorCreateUpdateDto dto);

    /// <summary>
    /// Удаляет запись посетителя.
    /// </summary>
    /// <param name="id">Идентификатор посетителя.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    bool Delete(int id);
}
