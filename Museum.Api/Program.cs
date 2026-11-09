using Museum.Application.Repositories;
using Museum.Application.Services;
using Museum.Api;
using Museum.Domain.Data;
using Museum.Domain.Entities;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilter>());

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

builder.Services.AddSingleton<IExcursionExhibitionRepository>(sp =>
    new InMemoryExcursionExhibitionRepository(sp.GetRequiredService<MuseumContext>().ExcursionExhibitions));

builder.Services.AddScoped<IVisitorService, VisitorService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IExcursionService, ExcursionService>();
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

app.MapControllers();

app.Run();
