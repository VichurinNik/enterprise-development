using Museum.Infrastructure.MySql;
using OpenTelemetry;

namespace Museum.Api;

/// <summary>
/// Настраивает подключение к MySQL и отправку телеметрии через Aspire.
/// </summary>
internal static class MuseumDatabaseConfiguration
{
    internal static void AddMuseumDatabase(this WebApplicationBuilder builder)
    {
        builder.AddMySqlDbContext<MuseumDbContext>("museum", settings =>
        {
            // Транзакция охватывает весь запрос. Повтор отдельной команды нарушил бы эту границу.
            settings.DisableRetry = true;
            settings.ServerVersion = "8.4.0-mysql";
        });

        if (!string.IsNullOrEmpty(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
    }
}
