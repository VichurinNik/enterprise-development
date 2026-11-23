using Museum.Domain.Entities;

namespace Museum.Domain.Repositories;

/// <summary>
/// Репозиторий связей между экскурсиями и выставками.
/// </summary>
public interface IExcursionExhibitionRepository
    : IRepository<ExcursionExhibition, (int ExcursionId, int ExhibitionId)>
{
    /// <summary>
    /// Проверяет, включена ли выставка хотя бы в одну экскурсию.
    /// </summary>
    /// <param name="exhibitionId">Идентификатор выставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если связь найдена.</returns>
    public Task<bool> ExistsForExhibitionAsync(int exhibitionId, CancellationToken cancellationToken);

    /// <summary>
    /// Заменяет связи экскурсии с выставками.
    /// </summary>
    /// <param name="excursion">Экскурсия, для которой обновляются связи.</param>
    /// <param name="exhibitions">Новый список выставок.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task ReplaceForExcursionAsync(Excursion excursion, IReadOnlyList<Exhibition> exhibitions, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет связи экскурсии с выставками.
    /// </summary>
    /// <param name="excursion">Экскурсия, для которой удаляются связи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task RemoveForExcursionAsync(Excursion excursion, CancellationToken cancellationToken);
}
