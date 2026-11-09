using Museum.Domain.Entities;

namespace Museum.Application.Repositories;

/// <summary>
/// Репозиторий связей между экскурсиями и выставками.
/// </summary>
public interface IExcursionExhibitionRepository
{
    /// <summary>
    /// Проверяет, включена ли выставка хотя бы в одну экскурсию.
    /// </summary>
    /// <param name="exhibitionId">Идентификатор выставки.</param>
    /// <returns>True, если связь найдена.</returns>
    bool ExistsForExhibition(int exhibitionId);

    /// <summary>
    /// Заменяет связи экскурсии с выставками.
    /// </summary>
    /// <param name="excursion">Экскурсия, для которой обновляются связи.</param>
    /// <param name="exhibitions">Новый список выставок.</param>
    void ReplaceForExcursion(Excursion excursion, IReadOnlyList<Exhibition> exhibitions);

    /// <summary>
    /// Удаляет связи экскурсии с выставками.
    /// </summary>
    /// <param name="excursion">Экскурсия, для которой удаляются связи.</param>
    void RemoveForExcursion(Excursion excursion);
}
