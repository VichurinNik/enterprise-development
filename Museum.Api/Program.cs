using Microsoft.EntityFrameworkCore;
using Museum.Domain.Repositories;
using Museum.Application.Services;
using Museum.Api;
using Museum.Infrastructure.MySql;
using Museum.Infrastructure.MySql.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
    options.Filters.Add<DatabaseTransactionFilter>();
});
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MuseumDbContext>((services, options) =>
{
    var connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("museum")
        ?? throw new InvalidOperationException(
            "Не задано подключение museum. Запустите Museum.AppHost или задайте ConnectionStrings__museum.");
    options.UseMySQL(connectionString);
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(MySqlRepository<>));
builder.Services.AddScoped<IExcursionExhibitionRepository, MySqlExcursionExhibitionRepository>();

builder.Services.AddScoped<IVisitorService, VisitorService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IExcursionService, ExcursionService>();
builder.Services.AddScoped<IExhibitionService, ExhibitionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddHealthChecks().AddCheck<MuseumDatabaseHealthCheck>("mysql");

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<MuseumDbContext>();
    await database.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Museum API Reference")
               .WithTheme(ScalarTheme.Moon);
    });
}

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

/// <summary>
/// Точка входа API музея для запуска приложения и интеграционных тестов.
/// </summary>
public partial class Program { }
