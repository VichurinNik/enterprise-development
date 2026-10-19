using Museum.Domain.Entities;

namespace Museum.Application.Repositories;

/// <summary>
/// Реализация универсального репозитория для хранения данных в оперативной памяти.
/// </summary>
/// <typeparam name="TEntity">Тип доменной сущности.</typeparam>
public class InMemoryRepository<TEntity>(List<TEntity> items) : IRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public IReadOnlyList<TEntity> GetAll() => items;

    /// <inheritdoc />
    public TEntity? GetById(int id) => items.FirstOrDefault(x => x.Id == id);

    /// <inheritdoc />
    public TEntity Create(TEntity entity)
    {
        entity.Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1;
        items.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public bool Update(TEntity entity)
    {
        var existing = GetById(entity.Id);
        if (existing is null)
        {
            return false;
        }

        var index = items.IndexOf(existing);
        items[index] = entity;
        return true;
    }

    /// <inheritdoc />
    public void Delete(int id)
    {
        var existing = GetById(id);
        if (existing is not null)
        {
            items.Remove(existing);
        }
    }
}