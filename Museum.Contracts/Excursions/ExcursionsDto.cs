
namespace Museum.Contracts.Excursions;
/// <summary>
/// DTO для возврата сведений об экскурсии.
/// </summary>
public record ExcursionsDto(
    int Id,
    DateTime Date,
    TimeSpan Duration,
    IReadOnlyList<int> ExhibitionIds
);