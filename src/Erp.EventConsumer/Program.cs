using Erp.Domain.Search;
using Erp.EventConsumer.Consumers;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using Erp.Infrastructure.Search;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(opt => opt.AddPolicy("AllowErpWeb", p =>
    p.WithOrigins(builder.Configuration["AllowedOrigin"] ?? "http://localhost:5173")
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()));

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddSingleton<ISearchService, MeilisearchService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PartyCreatedConsumer>();
    x.AddConsumer<PartyDeactivatedConsumer>();
    x.AddConsumer<PartyUpdatedConsumer>();
    x.AddConsumer<ArticleCreatedConsumer>();
    x.AddConsumer<ArticleUpdatedConsumer>();
    x.AddConsumer<ArticleDeactivatedConsumer>();
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderStatusChangedConsumer>();
    x.AddConsumer<QuoteCreatedConsumer>();
    x.AddConsumer<QuoteStatusChangedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"]);
        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.UseCors("AllowErpWeb");
app.MapHub<EventHub>("/hubs/events");

app.Run();
