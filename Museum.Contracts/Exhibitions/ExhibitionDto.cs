using Museum.Shared.Enums;

namespace Museum.Contracts.Exhibitions;

/// <summary>
/// DTO для возврата сведений о музейной выставке.
/// </summary>
/// <param name="Id">Идентификатор выставки.</param>
/// <param name="Name">Название выставки.</param>
/// <param name="Theme">Тематика выставки.</param>
/// <param name="HallNumber">Номер зала.</param>
/// <param name="StartDate">Дата начала работы.</param>
/// <param name="EndDate">Дата окончания работы.</param>
public record ExhibitionDto(
    int Id,
    string Name,
    ExhibitionTheme Theme,
    int HallNumber,
    DateTime StartDate,
    DateTime EndDate);
