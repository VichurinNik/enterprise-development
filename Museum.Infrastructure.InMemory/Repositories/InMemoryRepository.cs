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
    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetAll());
    }

    /// <inheritdoc />
    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetById(id));
    }

    /// <inheritdoc />
    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Create(entity));
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Update(entity));
    }

    /// <inheritdoc />
    public Task DeleteAsync(TKey id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Delete(id);
        return Task.CompletedTask;
    }

    /// <summary>Возвращает локальную коллекцию.</summary>
    protected IReadOnlyList<TEntity> GetAll() => items;

    /// <summary>Находит локальную запись по ключу.</summary>
    protected TEntity? GetById(TKey id) =>
        items.FirstOrDefault(item => EqualityComparer<TKey>.Default.Equals(getKey(item), id));

    /// <summary>Добавляет запись в коллекцию.</summary>
    protected virtual TEntity Create(TEntity entity)
    {
        if (GetById(getKey(entity)) is not null)
            throw new ArgumentException("Запись с таким ключом уже существует.", nameof(entity));

        items.Add(entity);
        return entity;
    }

    /// <summary>Заменяет существующую запись.</summary>
    protected virtual bool Update(TEntity entity)
    {
        var existing = GetById(getKey(entity));
        if (existing is null)
            return false;

        items[items.IndexOf(existing)] = entity;
        return true;
    }

    /// <summary>Удаляет запись из коллекции.</summary>
    protected virtual void Delete(TKey id)
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
    protected override TEntity Create(TEntity entity)
    {
        var entities = GetAll();
        entity.Id = entities.Count == 0 ? 1 : entities.Max(item => item.Id) + 1;
        return base.Create(entity);
    }
}
