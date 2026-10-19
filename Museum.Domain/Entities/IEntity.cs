namespace Museum.Domain.Entities;
/// <summary>
/// Базовый интерфейс доменной сущности с уникальным идентификатором.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    public int Id { get; set; }
}