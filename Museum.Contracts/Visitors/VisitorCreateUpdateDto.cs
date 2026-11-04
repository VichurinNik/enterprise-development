namespace Museum.Contracts.Visitors;

/// <summary>
/// DTO для создания или обновления посетителя музея.
/// </summary>
/// <param name="FullName">ФИО посетителя.</param>
/// <param name="Phone">Телефон.</param>
/// <param name="BirthDate">Дата рождения.</param>
public record VisitorCreateUpdateDto
    (
    string FullName,
    string Phone,
    DateOnly BirthDate
    );
