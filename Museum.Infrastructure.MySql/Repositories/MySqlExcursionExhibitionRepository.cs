using Microsoft.EntityFrameworkCore;
using Museum.Domain.Repositories;
using Museum.Domain.Entities;

namespace Museum.Infrastructure.MySql.Repositories;

/// <summary>
/// Хранит связи экскурсий с выставками в MySQL.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public sealed class MySqlExcursionExhibitionRepository(MuseumDbContext context)
    : IExcursionExhibitionRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<ExcursionExhibition>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.ExcursionExhibitions.OrderBy(link => link.ExcursionId).ThenBy(link => link.ExhibitionId)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<ExcursionExhibition?> GetByIdAsync(
        (int ExcursionId, int ExhibitionId) id, CancellationToken cancellationToken) =>
        await context.ExcursionExhibitions.FindAsync([id.ExcursionId, id.ExhibitionId], cancellationToken);

    /// <inheritdoc />
    public async Task<ExcursionExhibition> CreateAsync(ExcursionExhibition entity, CancellationToken cancellationToken)
    {
        context.ExcursionExhibitions.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(ExcursionExhibition entity, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync((entity.ExcursionId, entity.ExhibitionId), cancellationToken);
        if (existing is null)
            return false;

        context.Entry(existing).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task DeleteAsync((int ExcursionId, int ExhibitionId) id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return;

        context.ExcursionExhibitions.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsForExhibitionAsync(int exhibitionId, CancellationToken cancellationToken) =>
        context.ExcursionExhibitions.AnyAsync(link => link.ExhibitionId == exhibitionId, cancellationToken);

    /// <inheritdoc />
    public async Task ReplaceForExcursionAsync(Excursion excursion, IReadOnlyList<Exhibition> exhibitions, CancellationToken cancellationToken)
    {
        await context.Entry(excursion).Collection(item => item.Exhibitions).LoadAsync(cancellationToken);
        var selectedIds = exhibitions.Select(item => item.Id).ToHashSet();
        var existingIds = excursion.Exhibitions.Select(link => link.ExhibitionId).ToHashSet();
        var removed = excursion.Exhibitions.Where(link => !selectedIds.Contains(link.ExhibitionId)).ToList();
        context.ExcursionExhibitions.RemoveRange(removed);

        foreach (var exhibition in exhibitions.Where(item => !existingIds.Contains(item.Id)))
        {
            context.ExcursionExhibitions.Add(new ExcursionExhibition
            {
                ExcursionId = excursion.Id,
                Excursion = excursion,
                ExhibitionId = exhibition.Id,
                Exhibition = exhibition
            });
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveForExcursionAsync(Excursion excursion, CancellationToken cancellationToken)
    {
        var links = await context.ExcursionExhibitions
            .Where(link => link.ExcursionId == excursion.Id).ToListAsync(cancellationToken);
        context.ExcursionExhibitions.RemoveRange(links);
        await context.SaveChangesAsync(cancellationToken);
    }
}
