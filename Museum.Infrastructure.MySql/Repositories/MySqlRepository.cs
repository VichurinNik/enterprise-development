using Microsoft.EntityFrameworkCore;
using Museum.Domain.Repositories;
using Museum.Domain.Entities;

namespace Museum.Infrastructure.MySql.Repositories;

/// <summary>
/// Выполняет CRUD-операции с отслеживаемыми сущностями MySQL.
/// </summary>
/// <typeparam name="TEntity">Тип сущности с идентификатором.</typeparam>
/// <param name="context">Контекст базы в пределах одного запроса.</param>
public sealed class MySqlRepository<TEntity>(MuseumDbContext context) : IRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public IReadOnlyList<TEntity> GetAll() => Query().OrderBy(entity => entity.Id).ToList();

    /// <inheritdoc />
    public TEntity? GetById(int id) => Query().SingleOrDefault(entity => entity.Id == id);

    /// <inheritdoc />
    public TEntity Create(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
        context.SaveChanges();
        return entity;
    }

    /// <inheritdoc />
    public bool Update(TEntity entity)
    {
        var existing = context.Set<TEntity>().Find(entity.Id);
        if (existing is null)
            return false;

        context.Entry(existing).CurrentValues.SetValues(entity);
        context.SaveChanges();
        return true;
    }

    /// <inheritdoc />
    public void Delete(int id)
    {
        var entity = context.Set<TEntity>().Find(id);
        if (entity is null)
            return;

        context.Set<TEntity>().Remove(entity);
        context.SaveChanges();
    }

    private IQueryable<TEntity> Query()
    {
        IQueryable<TEntity> query = context.Set<TEntity>();
        if (typeof(TEntity) == typeof(Excursion))
            query = query.Include("Exhibitions.Exhibition").Include("Tickets").AsSplitQuery();
        else if (typeof(TEntity) == typeof(Ticket))
            query = query.Include("Excursion").Include("Visitor");
        return query;
    }
}
