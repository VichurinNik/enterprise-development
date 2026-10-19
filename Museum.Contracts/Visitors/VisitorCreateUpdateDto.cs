namespace Museum.Contracts.Visitors;

/// <summary>
/// DTO для создания или обновления посетителя музея.
/// </summary>
public record VisitorCreateUpdateDto
    (
    string FullName,
    string Phone,
    DateOnly BirthDate
    );