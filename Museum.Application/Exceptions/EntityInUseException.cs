namespace Museum.Application.Exceptions;

/// <summary>
/// Ошибка удаления записи, на которую ссылаются другие данные.
/// </summary>
/// <param name="message">Описание связи, препятствующей удалению.</param>
public sealed class EntityInUseException(string message) : Exception(message);
