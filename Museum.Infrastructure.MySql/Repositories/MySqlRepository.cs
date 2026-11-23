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
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken) =>
        await Query().OrderBy(entity => entity.Id).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await Query().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        context.Set<TEntity>().Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var existing = await context.Set<TEntity>().FindAsync([entity.Id], cancellationToken);
        if (existing is null)
            return false;

        context.Entry(existing).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await context.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity is null)
            return;

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TEntity> Query()
    {
        IQueryable<TEntity> query = context.Set<TEntity>();
        if (typeof(TEntity) == typeof(Excursion))
            query = query.Include("Exhibitions.Exhibition").Include("Tickets").AsSplitQuery();
        else if (typeof(TEntity) == typeof(Ticket))
            query = query.Include("Excursion.Exhibitions.Exhibition").Include("Visitor");
        return query;
    }
}
