using Museum.Domain.Entities;

namespace Museum.Domain.Repositories;

/// <summary>
/// Обобщённый интерфейс репозитория для выполнения CRUD-операций.
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности.</typeparam>
/// <typeparam name="TKey">Тип ключа сущности.</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class
{
    /// <summary>
    /// Возвращает список всех сущностей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает сущность по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт новую сущность.
    /// </summary>
    /// <param name="entity">Создаваемая сущность.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет существующую сущность.
    /// </summary>
    /// <param name="entity">Сущность с обновлёнными данными.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сущность по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task DeleteAsync(TKey id, CancellationToken cancellationToken);
}

/// <summary>
/// Репозиторий сущностей с целочисленным идентификатором.
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности.</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity
{
}
