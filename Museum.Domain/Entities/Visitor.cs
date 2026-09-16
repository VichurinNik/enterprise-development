namespace Museum.Domain.Entities;

public class Visitor
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}