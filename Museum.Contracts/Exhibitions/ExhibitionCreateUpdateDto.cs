using Museum.Shared.Enums;

namespace Museum.Contracts.Exhibitions;

/// <summary>
/// DTO для создания или обновления музейной выставки.
/// </summary>
/// <param name="Name">Название выставки.</param>
/// <param name="Theme">Тематика выставки.</param>
/// <param name="HallNumber">Номер зала.</param>
/// <param name="StartDate">Дата начала работы.</param>
/// <param name="EndDate">Дата окончания работы.</param>
public record ExhibitionCreateUpdateDto(
    string Name,
    ExhibitionTheme Theme,
    int HallNumber,
    DateTime StartDate,
    DateTime EndDate);
