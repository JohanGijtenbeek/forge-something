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

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]!);
            h.Password(builder.Configuration["RabbitMq:Password"]!);
        });
        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.UseCors("AllowErpWeb");
app.MapHub<EventHub>("/hubs/events");

app.Run();
