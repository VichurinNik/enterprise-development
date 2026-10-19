using Museum.Application.Repositories;
using Museum.Application.Services;
using Museum.Domain.Data;
using Museum.Domain.Entities;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSingleton(DataSeeder.Seed());


builder.Services.AddSingleton<IRepository<Exhibition>>(sp =>
    new InMemoryRepository<Exhibition>(sp.GetRequiredService<MuseumContext>().Exhibitions));

builder.Services.AddSingleton<IRepository<Excursion>>(sp =>
    new InMemoryRepository<Excursion>(sp.GetRequiredService<MuseumContext>().Excursions));

builder.Services.AddSingleton<IRepository<Visitor>>(sp =>
    new InMemoryRepository<Visitor>(sp.GetRequiredService<MuseumContext>().Visitors));

builder.Services.AddSingleton<IRepository<Ticket>>(sp =>
    new InMemoryRepository<Ticket>(sp.GetRequiredService<MuseumContext>().Tickets));


builder.Services.AddScoped<IExhibitionService, ExhibitionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Museum API Reference")
               .WithTheme(ScalarTheme.Moon);
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();