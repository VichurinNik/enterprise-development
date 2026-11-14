using Microsoft.EntityFrameworkCore;
using Museum.Application.Repositories;
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
    public bool ExistsForExhibition(int exhibitionId) =>
        context.ExcursionExhibitions.Any(link => link.ExhibitionId == exhibitionId);

    /// <inheritdoc />
    public void ReplaceForExcursion(Excursion excursion, IReadOnlyList<Exhibition> exhibitions)
    {
        context.Entry(excursion).Collection(item => item.Exhibitions).Load();
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

        context.SaveChanges();
    }

    /// <inheritdoc />
    public void RemoveForExcursion(Excursion excursion)
    {
        var links = context.ExcursionExhibitions.Where(link => link.ExcursionId == excursion.Id).ToList();
        context.ExcursionExhibitions.RemoveRange(links);
        context.SaveChanges();
    }
}
