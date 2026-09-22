namespace Museum.Domain.Entities;

/// <summary>
/// Представляет посетителя музея.
/// </summary>
public class Visitor
{
    /// <summary>
    /// Уникальный идентификатор посетителя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Фамилия, имя и отчество посетителя.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Номер телефона посетителя.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Дата рождения посетителя.
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Билеты, приобретённые посетителем.
    /// </summary>
    public List<Ticket> Tickets { get; set; } = new();
}