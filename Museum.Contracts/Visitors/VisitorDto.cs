namespace Museum.Contracts.Visitors;

/// <summary>
/// DTO для возврата сведений о посетителе музея.
/// </summary>
/// <param name="Id">Идентификатор посетителя.</param>
/// <param name="FullName">ФИО посетителя.</param>
/// <param name="Phone">Телефон.</param>
/// <param name="BirthDate">Дата рождения.</param>
public record VisitorDto
    (
    int Id,
    string FullName,
    string Phone,
    DateOnly BirthDate
    );
