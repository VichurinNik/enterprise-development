using Museum.Domain.Entities;

namespace Museum.Application.Repositories;

/// <summary>
/// Хранит связи экскурсий с выставками в памяти и поддерживает навигационные коллекции.
/// </summary>
/// <param name="items">Общая коллекция связей из контекста музея.</param>
public class InMemoryExcursionExhibitionRepository(List<ExcursionExhibition> items) : IExcursionExhibitionRepository
{
    /// <inheritdoc />
    public bool ExistsForExhibition(int exhibitionId) => items.Any(link => link.ExhibitionId == exhibitionId);

    /// <inheritdoc />
    public void ReplaceForExcursion(Excursion excursion, IReadOnlyList<Exhibition> exhibitions)
    {
        RemoveForExcursion(excursion);
        foreach (var exhibition in exhibitions)
        {
            var link = new ExcursionExhibition
            {
                ExcursionId = excursion.Id,
                Excursion = excursion,
                ExhibitionId = exhibition.Id,
                Exhibition = exhibition
            };
            items.Add(link);
            excursion.Exhibitions.Add(link);
            exhibition.ExcursionExhibitions.Add(link);
        }
    }

    /// <inheritdoc />
    public void RemoveForExcursion(Excursion excursion)
    {
        foreach (var link in items.Where(link => link.ExcursionId == excursion.Id).ToList())
        {
            link.Exhibition.ExcursionExhibitions.Remove(link);
            link.Excursion.Exhibitions.Remove(link);
            items.Remove(link);
        }
    }
}
