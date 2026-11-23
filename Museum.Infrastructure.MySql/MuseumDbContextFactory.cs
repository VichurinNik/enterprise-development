using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Museum.Infrastructure.MySql;

/// <summary>
/// Создаёт контекст для команд создания миграций без запуска Aspire.
/// </summary>
public sealed class MuseumDbContextFactory : IDesignTimeDbContextFactory<MuseumDbContext>
{
    /// <summary>
    /// Создаёт контекст с настройками MySQL для инструментов EF Core.
    /// </summary>
    /// <param name="args">Аргументы команды EF Core.</param>
    /// <returns>Контекст базы музея.</returns>
    public MuseumDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__museum")
            ?? "Server=localhost;Database=museum;User ID=root";
        var options = new DbContextOptionsBuilder<MuseumDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4))).Options;
        return new MuseumDbContext(options);
    }
}
