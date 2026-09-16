namespace Museum.Domain.Entities;

public class ExcursionExhibition
{
    public int ExcursionId { get; set; }

    public Excursion Excursion { get; set; } = null!;

    public int ExhibitionId { get; set; }

    public Exhibition Exhibition { get; set; } = null!;
}