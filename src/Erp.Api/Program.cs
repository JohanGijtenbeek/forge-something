using Erp.Api.Endpoints;
using Erp.Domain.Parties;
using Erp.Domain.Search;
using Erp.Infrastructure.Handlers;
using Erp.Infrastructure.Maintenance;
using Erp.Infrastructure.Persistence;
using Erp.Infrastructure.Repositories;
using Erp.Infrastructure.Search;
using Erp.Infrastructure.Snapshots;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "ERP API";
        document.Info.Version = "v1";
        document.Info.Description = "ERP systeem voor metaalbewerking";
        return Task.CompletedTask;
    });
});

// Request timeouts
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(10),
        TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
    };

    // Langere timeout voor zware operaties zoals reindex
    options.AddPolicy("long", new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(60),
        TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
    });
});

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter("concurrency", o =>
    {
        o.PermitLimit = 50;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 25;
    });

    options.AddSlidingWindowLimiter("sliding", o =>
    {
        o.PermitLimit = 300;
        o.Window = TimeSpan.FromMinutes(1);
        o.SegmentsPerWindow = 6;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 10;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "10";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Te veel requests. Probeer het over enkele seconden opnieuw.",
            retryAfterSeconds = 10
        }, ct);
    };
});

// Response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Problem Details
builder.Services.AddProblemDetails();

// Health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sqlserver",
        tags: ["db", "sql"])
    .AddUrlGroup(
        new Uri($"{builder.Configuration["Meilisearch:Url"]}/health"),
        name: "meilisearch",
        tags: ["search"]);

// Dapper
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<IPartyRepository, PartyRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrganizationHandler).Assembly));

// Search
builder.Services.AddSingleton<ISearchService, MeilisearchService>();

// Snapshots
builder.Services.AddScoped<SnapshotService>();

// Maintenance job
builder.Services.AddHostedService<MaintenanceJob>();

// CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.UseResponseCompression();
app.UseRequestTimeouts();
app.UseRateLimiter();

// OpenAPI + Scalar
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "ERP API";
    options.Theme = ScalarTheme.DeepSpace;
});

// Health checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
    }
});

// Meilisearch initialiseren en seeden bij startup
var search = app.Services.GetRequiredService<ISearchService>();
await search.InitializeAsync();

using (var startupScope = app.Services.CreateScope())
{
    var repo = startupScope.ServiceProvider.GetRequiredService<IPartyRepository>();
    await search.ReindexPartiesAsync(repo);
}

// Endpoints
app.MapPartyEndpoints();
app.MapSearchEndpoints();

app.Run();
