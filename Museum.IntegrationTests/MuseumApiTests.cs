using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Museum.Contracts.Analytics;
using Museum.Contracts.Excursions;
using Museum.Contracts.Exhibitions;
using Museum.Contracts.Tickets;
using Museum.Contracts.Visitors;
using Museum.Infrastructure.MySql;
using Museum.Shared.Enums;
using MySql.Data.MySqlClient;

namespace Museum.IntegrationTests;

public sealed class MuseumApiTests : IAsyncLifetime
{
    private readonly string databaseName = $"museum_tests_{Guid.NewGuid():N}";
    private string serverConnection = "";
    private string databaseConnection = "";
    private MuseumFactory factory = null!;
    private HttpClient client = null!;

    public async ValueTask InitializeAsync()
    {
        serverConnection = Environment.GetEnvironmentVariable("MYSQL_TEST_CONNECTION")
            ?? throw new InvalidOperationException(
                "Для интеграционных тестов задайте MYSQL_TEST_CONNECTION к тестовому серверу MySQL.");
        var builder = new MySqlConnectionStringBuilder(serverConnection) { Database = "" };
        serverConnection = builder.ConnectionString;
        await using var connection = new MySqlConnection(serverConnection);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE `{databaseName}` CHARACTER SET utf8mb4";
        await command.ExecuteNonQueryAsync();
        builder.Database = databaseName;
        databaseConnection = builder.ConnectionString;
        factory = new MuseumFactory(databaseConnection);
        client = factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        client?.Dispose();
        if (factory is not null)
            await factory.DisposeAsync();

        if (serverConnection.Length == 0)
            return;

        MySqlConnection.ClearAllPools();
        await using var connection = new MySqlConnection(serverConnection);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"DROP DATABASE IF EXISTS `{databaseName}`";
        await command.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task Migrations_CreateTablesAndSeedOnlyOnce()
    {
        using var scope = factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<MuseumDbContext>();
        await database.Database.MigrateAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, (await database.Database.GetAppliedMigrationsAsync(cancellationToken: TestContext.Current.CancellationToken)).Count());
        Assert.Empty(await database.Database.GetPendingMigrationsAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(10, await database.Exhibitions.CountAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(10, await database.Excursions.CountAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(10, await database.Visitors.CountAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(30, await database.Tickets.CountAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(20, await database.ExcursionExhibitions.CountAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health", cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
    }

    [Fact]
    public async Task Crud_PersistsChangesAndProtectsRelations()
    {
        var visitor = await Post<VisitorDto>("/api/visitors",
            new VisitorCreateUpdateDto("Новый посетитель", "+79001234567", new DateOnly(2000, 1, 1)));
        var exhibition = await Post<ExhibitionDto>("/api/exhibitions",
            new ExhibitionCreateUpdateDto("Новая выставка", ExhibitionTheme.Art, 2,
                new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)));
        var excursion = await Post<ExcursionsDto>("/api/excursions",
            new ExcursionCreateUpdateDto(new DateTime(2026, 11, 15, 12, 0, 0),
                TimeSpan.FromMinutes(90), [exhibition.Id, 1]));
        Assert.Equal(new[] { 1, exhibition.Id }, excursion.ExhibitionIds.Order().ToArray());

        var ticket = await Post<TicketDto>("/api/tickets",
            new TicketCreateUpdateDto(excursion.Id, visitor.Id, TicketType.Adult, 1234.56m));

        foreach (var route in new[] { $"visitors/{visitor.Id}", $"excursions/{excursion.Id}", $"exhibitions/{exhibition.Id}" })
            Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync($"/api/{route}", cancellationToken: TestContext.Current.CancellationToken)).StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/excursions/{excursion.Id}",
            new ExcursionCreateUpdateDto(excursion.Date, TimeSpan.FromMinutes(120), [exhibition.Id, 2]), cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
        var updatedExcursion = await client.GetFromJsonAsync<ExcursionsDto>($"/api/excursions/{excursion.Id}", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 2, exhibition.Id }, updatedExcursion!.ExhibitionIds.Order().ToArray());
        Assert.Equal(TimeSpan.FromMinutes(120), updatedExcursion.Duration);

        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/tickets/{ticket.Id}",
            new TicketCreateUpdateDto(1, 1, TicketType.Discounted, 500m), cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
        var updatedTicket = await client.GetFromJsonAsync<TicketDto>($"/api/tickets/{ticket.Id}", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(1, updatedTicket!.ExcursionId);
        Assert.Equal(1, updatedTicket.VisitorId);
        Assert.Equal(500m, updatedTicket.Price);

        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/visitors/{visitor.Id}",
            new VisitorCreateUpdateDto("Изменённое имя", "+79007654321", new DateOnly(2001, 2, 3)), cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal("Изменённое имя",
            (await client.GetFromJsonAsync<VisitorDto>($"/api/visitors/{visitor.Id}", cancellationToken: TestContext.Current.CancellationToken))!.FullName);

        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/exhibitions/{exhibition.Id}",
            new ExhibitionCreateUpdateDto("Изменённая выставка", ExhibitionTheme.History, 3,
                new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)), cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal("Изменённая выставка",
            (await client.GetFromJsonAsync<ExhibitionDto>($"/api/exhibitions/{exhibition.Id}", cancellationToken: TestContext.Current.CancellationToken))!.Name);

        foreach (var route in new[] { $"tickets/{ticket.Id}", $"excursions/{excursion.Id}", $"exhibitions/{exhibition.Id}", $"visitors/{visitor.Id}" })
        {
            Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/{route}", cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/{route}", cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
        }
    }

    [Fact]
    public async Task InvalidRequests_DoNotChangeExistingData()
    {
        var original = await client.GetFromJsonAsync<TicketDto>("/api/tickets/1", cancellationToken: TestContext.Current.CancellationToken);
        var response = await client.PutAsJsonAsync("/api/tickets/1",
            new TicketCreateUpdateDto(1, int.MaxValue, TicketType.Adult, 999m), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(original, await client.GetFromJsonAsync<TicketDto>("/api/tickets/1", cancellationToken: TestContext.Current.CancellationToken));

        var excursion = await client.GetFromJsonAsync<ExcursionsDto>("/api/excursions/1", cancellationToken: TestContext.Current.CancellationToken);
        response = await client.PutAsJsonAsync("/api/excursions/1",
            new ExcursionCreateUpdateDto(DateTime.Today, TimeSpan.FromHours(2), [int.MaxValue]), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var unchanged = await client.GetFromJsonAsync<ExcursionsDto>("/api/excursions/1", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(excursion!.Date, unchanged!.Date);
        Assert.Equal(excursion.ExhibitionIds, unchanged.ExhibitionIds);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.GetAsync("/api/analytics/theme-summary?start=2026-12-31&end=2026-01-01", cancellationToken: TestContext.Current.CancellationToken)).StatusCode);
    }

    [Fact]
    public async Task Analytics_ReturnsExpectedResultsFromMySql()
    {
        var top = await client.GetFromJsonAsync<List<TopExhibitionDto>>("/api/analytics/top-exhibitions", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 1, 10, 2, 3, 4 }, top!.Select(item => item.ExhibitionId));
        Assert.All(top!, item => Assert.Equal(5, item.VisitorCount));

        var minimum = await client.GetFromJsonAsync<List<ExcursionParticipantsDto>>("/api/analytics/minimum-participants", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(10, minimum!.Count);
        Assert.All(minimum, item => Assert.Equal(3, item.ParticipantCount));

        var summary = await client.GetFromJsonAsync<List<ThemeAttendanceSummaryDto>>(
            "/api/analytics/theme-summary?start=2026-01-01&end=2026-12-31", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 10000m, 12500m, 12500m, 10000m }, summary!.Select(item => item.TotalRevenue));
        Assert.Equal(new[] { 9, 10, 10, 9 }, summary!.Select(item => item.TotalVisitors));

        var hall = await client.GetFromJsonAsync<List<ExcursionsDto>>(
            "/api/analytics/hall-excursions?hallNumber=1&start=2026-01-01&end=2026-12-31", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 1, 3, 4, 6, 7, 9 }, hall!.Select(item => item.Id));

        var visitors = await client.GetFromJsonAsync<List<VisitorDto>>("/api/analytics/excursions/1/visitors", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 2, 3, 4 }, visitors!.Select(item => item.Id));
    }

    [Fact]
    public async Task Restart_PreservesCreatedData()
    {
        var visitor = await Post<VisitorDto>("/api/visitors",
            new VisitorCreateUpdateDto("Сохранённый посетитель", "+79001234567", new DateOnly(1995, 3, 1)));
        client.Dispose();
        await factory.DisposeAsync();
        factory = new MuseumFactory(databaseConnection);
        client = factory.CreateClient();
        Assert.Equal(visitor, await client.GetFromJsonAsync<VisitorDto>($"/api/visitors/{visitor.Id}", cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(11, (await client.GetFromJsonAsync<List<VisitorDto>>("/api/visitors", cancellationToken: TestContext.Current.CancellationToken))!.Count);
    }

    [Fact]
    public async Task FailedRequest_RollsBackAlreadySavedChanges()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/visitors")
        {
            Content = JsonContent.Create(new VisitorCreateUpdateDto(
                "Отменённая запись", "+79001234567", new DateOnly(2000, 1, 1)))
        };
        request.Headers.Add("X-Test-Fail-After-Save", "true");
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var visitors = await client.GetFromJsonAsync<List<VisitorDto>>(
            "/api/visitors", TestContext.Current.CancellationToken);
        Assert.Equal(10, visitors!.Count);
        Assert.DoesNotContain(visitors, item => item.FullName == "Отменённая запись");
    }
    private async Task<T> Post<T>(string route, object value)
    {
        var response = await client.PostAsJsonAsync(route, value, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: TestContext.Current.CancellationToken))!;
    }

    private sealed class FailAfterSaveFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if (result.Exception is null && context.HttpContext.Request.Headers.ContainsKey("X-Test-Fail-After-Save"))
                throw new InvalidOperationException("Ошибка после сохранения для проверки отката транзакции.");
        }
    }
    private sealed class MuseumFactory(string connectionString) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services => services.Configure<MvcOptions>(
                options => options.Filters.Add<FailAfterSaveFilter>(order: 1)));
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?> { ["ConnectionStrings:museum"] = connectionString }));
        }
    }
}
