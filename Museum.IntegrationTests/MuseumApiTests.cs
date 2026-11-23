using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Museum.Contracts.Analytics;
using Museum.Contracts.Excursions;
using Museum.Contracts.Exhibitions;
using Museum.Contracts.Tickets;
using Museum.Contracts.Visitors;
using Museum.Infrastructure.MySql;
using Museum.Shared.Enums;

namespace Museum.IntegrationTests;

public sealed class MuseumApiTests : IAsyncLifetime
{
    private DistributedApplication? app;
    private string databaseConnection = null!;
    private HttpClient client = null!;

    public async ValueTask InitializeAsync()
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(3));

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Museum_AppHost>(
            ["Testing:DisableDataVolume=true"], timeout.Token);
        app = await builder.BuildAsync(timeout.Token);
        await app.StartAsync(timeout.Token);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("api", timeout.Token);
        databaseConnection = await app.GetConnectionStringAsync("museum", timeout.Token)
            ?? throw new InvalidOperationException("Aspire не передал подключение к тестовой базе.");
        client = app.CreateHttpClient("api");
    }

    public async ValueTask DisposeAsync()
    {
        client?.Dispose();
        if (app is not null)
            await app.DisposeAsync();
    }

    private MuseumDbContext CreateDatabaseContext() => new(
        new DbContextOptionsBuilder<MuseumDbContext>()
            .UseMySql(databaseConnection, new MySqlServerVersion(new Version(8, 4))).Options);

    [Fact]
    public async Task Migrations_CreateTablesAndSeedOnlyOnce()
    {
        await using var database = CreateDatabaseContext();
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

        Assert.Equal(visitor, ticket.Visitor);
        Assert.Equal(excursion.Id, ticket.Excursion.Id);
        Assert.Contains(ticket.Exhibitions, item => item.Id == exhibition.Id && item.Theme == "Art");
        Assert.Equal(TicketType.Adult, ticket.TicketType);

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
        var unchangedTicket = await client.GetFromJsonAsync<TicketDto>("/api/tickets/1",
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(original);
        Assert.NotNull(unchangedTicket);
        Assert.Equal(original.Visitor, unchangedTicket.Visitor);
        Assert.Equal(original.Excursion.Id, unchangedTicket.Excursion.Id);
        Assert.Equal(original.Price, unchangedTicket.Price);
        Assert.Equal(original.TicketType, unchangedTicket.TicketType);

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
        Assert.Equal(new[] { "History", "Art", "Archaeology", "Science" }, summary!.Select(item => item.Theme));
        Assert.Equal(new[] { 10000m, 12500m, 12500m, 10000m }, summary!.Select(item => item.TotalRevenue));
        Assert.Equal(new[] { 9, 10, 10, 9 }, summary!.Select(item => item.TotalVisitors));

        var hall = await client.GetFromJsonAsync<List<ExcursionsDto>>(
            "/api/analytics/hall-excursions?hallNumber=1&start=2026-01-01&end=2026-12-31", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 1, 3, 4, 6, 7, 9 }, hall!.Select(item => item.Id));

        var visitors = await client.GetFromJsonAsync<List<VisitorDto>>("/api/analytics/excursions/1/visitors", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(new[] { 2, 3, 4 }, visitors!.Select(item => item.Id));
    }

    [Fact]
    public async Task SecondApiStartup_PreservesCreatedData()
    {
        var visitor = await Post<VisitorDto>("/api/visitors",
            new VisitorCreateUpdateDto("Сохранённый посетитель", "+79001234567", new DateOnly(1995, 3, 1)));
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        var commands = app!.Services.GetRequiredService<ResourceCommandService>();
        var result = await commands.ExecuteCommandAsync("api-second", KnownResourceCommands.StartCommand, timeout.Token);
        Assert.True(result.Success, result.Message);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("api-second", timeout.Token);
        using var secondClient = app.CreateHttpClient("api-second");
        Assert.Equal(visitor, await secondClient.GetFromJsonAsync<VisitorDto>($"/api/visitors/{visitor.Id}", timeout.Token));
        Assert.Equal(11, (await secondClient.GetFromJsonAsync<List<VisitorDto>>("/api/visitors", timeout.Token))!.Count);
    }

    [Fact]
    public async Task FailedRequest_RollsBackAlreadySavedChanges()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var database = CreateDatabaseContext();
        await database.Database.ExecuteSqlRawAsync(
            """
            CREATE TRIGGER RejectExcursionLink
            BEFORE INSERT ON ExcursionExhibitions
            FOR EACH ROW
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Integration test rejected the link'
            """, cancellationToken);

        using var response = await client.PostAsJsonAsync("/api/excursions",
            new ExcursionCreateUpdateDto(new DateTime(2026, 11, 15, 12, 0, 0),
                TimeSpan.FromMinutes(60), [1]), cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(10, await database.Excursions.CountAsync(cancellationToken));
        Assert.Equal(20, await database.ExcursionExhibitions.CountAsync(cancellationToken));
    }

    private async Task<T> Post<T>(string route, object value)
    {
        var response = await client.PostAsJsonAsync(route, value, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: TestContext.Current.CancellationToken))!;
    }

}
