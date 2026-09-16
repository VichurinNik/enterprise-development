using Museum.Domain.Enums;

namespace Museum.Domain.Entities;

public class Exhibition
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ExhibitionTheme Theme { get; set; }

    public int HallNumber { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<ExcursionExhibition> ExcursionExhibitions { get; set; } = new();
}