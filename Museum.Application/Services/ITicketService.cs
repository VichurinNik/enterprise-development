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
    /// <returns>Список данных билетов.</returns>
    IReadOnlyList<TicketDto> GetAll();

    /// <summary>
    /// Возвращает данные билета по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <returns>Данные билета или null, если запись не найдена.</returns>
    TicketDto? GetById(int id);

    /// <summary>
    /// Создаёт запись билета.
    /// </summary>
    /// <param name="dto">Данные для создания билета.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    TicketDto Create(TicketCreateUpdateDto dto);

    /// <summary>
    /// Обновляет данные билета.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <param name="dto">Новые данные билета.</param>
    /// <returns>True, если запись обновлена; false, если она не найдена.</returns>
    bool Update(int id, TicketCreateUpdateDto dto);

    /// <summary>
    /// Удаляет запись билета.
    /// </summary>
    /// <param name="id">Идентификатор билета.</param>
    /// <returns>True, если запись удалена; false, если она не найдена.</returns>
    bool Delete(int id);
}
