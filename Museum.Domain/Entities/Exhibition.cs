using Museum.Domain.Enums;

namespace Museum.Domain.Entities;

/// <summary>
/// Представляет музейную выставку.
/// </summary>
public class Exhibition
{
    /// <summary>
    /// Уникальный идентификатор выставки.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название выставки.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Тематика выставки.
    /// </summary>
    public ExhibitionTheme Theme { get; set; }

    /// <summary>
    /// Номер зала, в котором проходит выставка.
    /// </summary>
    public int HallNumber { get; set; }

    /// <summary>
    /// Дата начала работы выставки.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Дата окончания работы выставки.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Связи выставки с экскурсиями.
    /// </summary>
    public List<ExcursionExhibition> ExcursionExhibitions { get; set; } = new();
}