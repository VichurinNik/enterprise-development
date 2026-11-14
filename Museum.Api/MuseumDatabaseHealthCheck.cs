using Microsoft.Extensions.Diagnostics.HealthChecks;
using Museum.Infrastructure.MySql;

namespace Museum.Api;

/// <summary>
/// Проверяет доступность базы музея для оркестратора.
/// </summary>
/// <param name="scopeFactory">Фабрика областей зависимостей для проверки контекста.</param>
public sealed class MuseumDatabaseHealthCheck(IServiceScopeFactory scopeFactory) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<MuseumDbContext>();
        return await database.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("База данных музея недоступна.");
    }
}
