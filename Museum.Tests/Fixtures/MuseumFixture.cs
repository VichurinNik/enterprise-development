using Museum.Domain.Data;

namespace Museum.Tests.Fixtures;

public class MuseumFixture
{
    public MuseumContext Context { get; }

    public MuseumFixture()
    {
        Context = DataSeeder.Seed();
    }
}