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
    public IReadOnlyList<TEntity> GetAll();

    /// <summary>
    /// Возвращает сущность по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    public TEntity? GetById(TKey id);

    /// <summary>
    /// Создаёт новую сущность.
    /// </summary>
    /// <param name="entity">Создаваемая сущность.</param>
    public TEntity Create(TEntity entity);

    /// <summary>
    /// Обновляет существующую сущность.
    /// </summary>
    /// <param name="entity">Сущность с обновлёнными данными.</param>
    public bool Update(TEntity entity);

    /// <summary>
    /// Удаляет сущность по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой сущности.</param>
    public void Delete(TKey id);
}

/// <summary>
/// Репозиторий сущностей с целочисленным идентификатором.
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности.</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity
{
}
