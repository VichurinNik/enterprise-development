using Museum.Domain.Data;

namespace Museum.Tests.Fixtures;

/// <summary>
/// Предоставляет подготовленный тестовый контекст музея.
/// </summary>
public class MuseumFixture
{
    /// <summary>
    /// Контекст музея с подготовленными тестовыми данными.
    /// </summary>
    public MuseumContext Context { get; }

    /// <summary>
    /// Создаёт фикстуру и заполняет контекст тестовыми данными.
    /// </summary>
    public MuseumFixture()
    {
        Context = DataSeeder.Seed();
    }
}