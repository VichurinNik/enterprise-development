using Museum.Domain.Enums;

namespace Museum.Contracts.Exhibitions;

/// <summary>
/// DTO для создания или обновления музейной выставки.
/// </summary>
public record ExhibitionCreateUpdateDto(
    string Name,
    ExhibitionTheme Theme,
    int HallNumber,
    DateTime StartDate,
    DateTime EndDate);