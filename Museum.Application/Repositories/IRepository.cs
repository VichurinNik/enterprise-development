using Museum.Domain.Entities;

namespace Museum.Application.Repositories;

/// <summary>
/// Обобщённый интерфейс репозитория для выполнения CRUD-операций.
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности.</typeparam>
public interface IRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Возвращает список всех сущностей.
    /// </summary>
    public IReadOnlyList<TEntity> GetAll();

    /// <summary>
    /// Возвращает сущность по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    public TEntity? GetById(int id);

    /// <summary>
    /// Создаёт новую сущность и присваивает ей идентификатор.
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
    public void Delete(int id);
}
