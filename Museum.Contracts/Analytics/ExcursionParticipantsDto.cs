namespace Museum.Contracts.Analytics;

/// <summary>
/// Экскурсия и количество её участников.
/// </summary>
/// <param name="ExcursionId">Идентификатор экскурсии.</param>
/// <param name="ParticipantCount">Количество уникальных участников.</param>
public record ExcursionParticipantsDto(int ExcursionId, int ParticipantCount);
