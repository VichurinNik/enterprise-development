using System;
using System.Collections.Generic;

namespace Museum.Domain.Entities;

/// <summary>
/// Представляет экскурсию в музее.
/// </summary>
public class Excursion
{
    /// <summary>
    /// Уникальный идентификатор экскурсии.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Дата проведения экскурсии.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Время начала экскурсии.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Продолжительность экскурсии.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Выставки, которые посещаются во время экскурсии.
    /// </summary>
    public List<ExcursionExhibition> Exhibitions { get; set; } = new();

    /// <summary>
    /// Билеты, приобретённые на экскурсию.
    /// </summary>
    public List<Ticket> Tickets { get; set; } = new();
}