using Museum.Domain.Entities;
using Museum.Domain.Repositories;

namespace Museum.Infrastructure.InMemory.Repositories;

/// <summary>
/// Хранит сущности в коллекции и находит их по ключу.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TKey">Тип ключа.</typeparam>
/// <param name="items">Коллекция сущностей.</param>
/// <param name="getKey">Функция получения ключа сущности.</param>
public class InMemoryRepository<TEntity, TKey>(List<TEntity> items, Func<TEntity, TKey> getKey)
    : IRepository<TEntity, TKey> where TEntity : class
{
    /// <inheritdoc />
    public IReadOnlyList<TEntity> GetAll() => items;

    /// <inheritdoc />
    public TEntity? GetById(TKey id) =>
        items.FirstOrDefault(item => EqualityComparer<TKey>.Default.Equals(getKey(item), id));

    /// <inheritdoc />
    public virtual TEntity Create(TEntity entity)
    {
        if (GetById(getKey(entity)) is not null)
            throw new ArgumentException("Запись с таким ключом уже существует.", nameof(entity));

        items.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public virtual bool Update(TEntity entity)
    {
        var existing = GetById(getKey(entity));
        if (existing is null)
            return false;

        items[items.IndexOf(existing)] = entity;
        return true;
    }

    /// <inheritdoc />
    public virtual void Delete(TKey id)
    {
        if (GetById(id) is { } existing)
            items.Remove(existing);
    }
}

/// <summary>
/// Хранит сущности с целочисленным идентификатором и назначает идентификаторы новым записям.
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности.</typeparam>
/// <param name="items">Коллекция сущностей.</param>
public class InMemoryRepository<TEntity>(List<TEntity> items)
    : InMemoryRepository<TEntity, int>(items, entity => entity.Id), IRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public override TEntity Create(TEntity entity)
    {
        var entities = GetAll();
        entity.Id = entities.Count == 0 ? 1 : entities.Max(item => item.Id) + 1;
        return base.Create(entity);
    }
}
