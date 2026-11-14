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
            .UseMySQL(connectionString).Options;
        return new MuseumDbContext(options);
    }
}
