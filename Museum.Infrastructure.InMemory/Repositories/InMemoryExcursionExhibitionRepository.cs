using Museum.Domain.Entities;
using Museum.Domain.Repositories;

namespace Museum.Infrastructure.InMemory.Repositories;

/// <summary>
/// Хранит связи экскурсии с выставками по составному ключу.
/// </summary>
/// <param name="items">Коллекция связей.</param>
public sealed class InMemoryExcursionExhibitionRepository(List<ExcursionExhibition> items)
    : InMemoryRepository<ExcursionExhibition, (int ExcursionId, int ExhibitionId)>(
        items, link => (link.ExcursionId, link.ExhibitionId)), IExcursionExhibitionRepository
{
    /// <inheritdoc />
    protected override ExcursionExhibition Create(ExcursionExhibition entity)
    {
        var created = base.Create(entity);
        Attach(created);
        return created;
    }

    /// <inheritdoc />
    protected override bool Update(ExcursionExhibition entity)
    {
        var existing = GetById((entity.ExcursionId, entity.ExhibitionId));
        if (existing is null)
            return false;

        Detach(existing);
        base.Update(entity);
        Attach(entity);
        return true;
    }

    /// <inheritdoc />
    protected override void Delete((int ExcursionId, int ExhibitionId) id)
    {
        if (GetById(id) is { } existing)
            Detach(existing);

        base.Delete(id);
    }

    /// <inheritdoc />
    public Task<bool> ExistsForExhibitionAsync(int exhibitionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetAll().Any(link => link.ExhibitionId == exhibitionId));
    }

    /// <inheritdoc />
    public Task ReplaceForExcursionAsync(Excursion excursion, IReadOnlyList<Exhibition> exhibitions,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RemoveForExcursion(excursion);
        foreach (var exhibition in exhibitions)
        {
            Create(new ExcursionExhibition
            {
                ExcursionId = excursion.Id,
                Excursion = excursion,
                ExhibitionId = exhibition.Id,
                Exhibition = exhibition
            });
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveForExcursionAsync(Excursion excursion, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RemoveForExcursion(excursion);
        return Task.CompletedTask;
    }

    private void RemoveForExcursion(Excursion excursion)
    {
        foreach (var link in GetAll().Where(link => link.ExcursionId == excursion.Id).ToArray())
            Delete((link.ExcursionId, link.ExhibitionId));
    }

    private static void Attach(ExcursionExhibition link)
    {
        link.Excursion.Exhibitions.Add(link);
        link.Exhibition.ExcursionExhibitions.Add(link);
    }

    private static void Detach(ExcursionExhibition link)
    {
        link.Excursion.Exhibitions.Remove(link);
        link.Exhibition.ExcursionExhibitions.Remove(link);
    }
}
