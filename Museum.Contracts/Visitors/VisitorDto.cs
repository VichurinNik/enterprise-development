namespace Museum.Contracts.Visitors;

/// <summary>
/// DTO для возврата сведений о посетителе музея.
/// </summary>
public record VisitorDto
    (
    int Id,
    string FullName,
    string Phone,
    DateOnly BirthDate
    );