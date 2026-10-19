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
    public TEntity? GetById(int id);

    /// <summary>
    /// Создаёт новую сущность и присваивает ей идентификатор.
    /// </summary>
    public TEntity Create(TEntity entity);

    /// <summary>
    /// Обновляет существующую сущность.
    /// </summary>
    public bool Update(TEntity entity);

    /// <summary>
    /// Удаляет сущность по её идентификатору.
    /// </summary>
    public void Delete(int id);
}