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
    public override ExcursionExhibition Create(ExcursionExhibition entity)
    {
        var created = base.Create(entity);
        Attach(created);
        return created;
    }

    /// <inheritdoc />
    public override bool Update(ExcursionExhibition entity)
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
    public override void Delete((int ExcursionId, int ExhibitionId) id)
    {
        if (GetById(id) is { } existing)
            Detach(existing);

        base.Delete(id);
    }

    /// <inheritdoc />
    public bool ExistsForExhibition(int exhibitionId) =>
        GetAll().Any(link => link.ExhibitionId == exhibitionId);

    /// <inheritdoc />
    public void ReplaceForExcursion(Excursion excursion, IReadOnlyList<Exhibition> exhibitions)
    {
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
    }

    /// <inheritdoc />
    public void RemoveForExcursion(Excursion excursion)
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
