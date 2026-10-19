using Museum.Domain.Enums;

namespace Museum.Contracts.Exhibitions;

/// <summary>
/// DTO для возврата сведений о музейной выставке.
/// </summary>
public record ExhibitionDto(
    int Id,
    string Name,
    ExhibitionTheme Theme,
    int HallNumber,
    DateTime StartDate,
    DateTime EndDate);