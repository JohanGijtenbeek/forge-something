using Dapper;
using Erp.Domain.Parties.Events;
using Erp.Domain.Search;
using Erp.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Erp.Infrastructure.Handlers;

// ============================================================
// Search indexering
// Wordt aangeroepen na elk PartyCreated event
// ============================================================

public class IndexPartyOnCreatedHandler : INotificationHandler<PartyCreatedEvent>
{
    private readonly ISearchService _search;
    private readonly ILogger<IndexPartyOnCreatedHandler> _logger;

    public IndexPartyOnCreatedHandler(ISearchService search, ILogger<IndexPartyOnCreatedHandler> logger)
    {
        _search = search;
        _logger = logger;
    }

    public async Task Handle(PartyCreatedEvent notification, CancellationToken ct)
    {
        var roles = new List<string>();
        if (notification.IsCustomer) roles.Add("customer");
        if (notification.IsSupplier) roles.Add("supplier");

        await _search.IndexPartyAsync(new PartySearchDocument(
            notification.PartyId.ToString(),
            notification.Name,
            null, null, null,
            [.. roles],
            true
        ));

        _logger.LogInformation("Party '{Name}' geïndexeerd in Meilisearch", notification.Name);
    }
}

public class RemovePartyFromIndexOnDeactivatedHandler : INotificationHandler<PartyDeactivatedEvent>
{
    private readonly ISearchService _search;
    private readonly ILogger<RemovePartyFromIndexOnDeactivatedHandler> _logger;

    public RemovePartyFromIndexOnDeactivatedHandler(ISearchService search, ILogger<RemovePartyFromIndexOnDeactivatedHandler> logger)
    {
        _search = search;
        _logger = logger;
    }

    public async Task Handle(PartyDeactivatedEvent notification, CancellationToken ct)
    {
        await _search.DeletePartyAsync(notification.PartyId.ToString());
        _logger.LogInformation("Party '{Name}' verwijderd uit Meilisearch index", notification.Name);
    }
}

// ============================================================
// Audit logging
// Elke mutatie wordt weggeschreven naar de audit_log tabel
// ============================================================

public class AuditPartyCreatedHandler : INotificationHandler<PartyCreatedEvent>
{
    private readonly DbConnectionFactory _factory;

    public AuditPartyCreatedHandler(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(PartyCreatedEvent notification, CancellationToken ct)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at)
            VALUES (@AggregateId, @AggregateType, @EventType, @Payload, @OccurredAt)",
            new
            {
                AggregateId = notification.PartyId,
                AggregateType = "Party",
                EventType = "PartyCreated",
                Payload = System.Text.Json.JsonSerializer.Serialize(notification),
                OccurredAt = notification.OccurredAt
            });
    }
}

public class AuditPartyDeactivatedHandler : INotificationHandler<PartyDeactivatedEvent>
{
    private readonly DbConnectionFactory _factory;

    public AuditPartyDeactivatedHandler(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(PartyDeactivatedEvent notification, CancellationToken ct)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at)
            VALUES (@AggregateId, @AggregateType, @EventType, @Payload, @OccurredAt)",
            new
            {
                AggregateId = notification.PartyId,
                AggregateType = "Party",
                EventType = "PartyDeactivated",
                Payload = System.Text.Json.JsonSerializer.Serialize(notification),
                OccurredAt = notification.OccurredAt
            });
    }
}

public class AuditPartyUpdatedHandler : INotificationHandler<PartyUpdatedEvent>
{
    private readonly DbConnectionFactory _factory;

    public AuditPartyUpdatedHandler(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(PartyUpdatedEvent notification, CancellationToken ct)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at)
            VALUES (@AggregateId, @AggregateType, @EventType, @Payload, @OccurredAt)",
            new
            {
                AggregateId = notification.PartyId,
                AggregateType = "Party",
                EventType = "PartyUpdated",
                Payload = System.Text.Json.JsonSerializer.Serialize(notification),
                OccurredAt = notification.OccurredAt
            });
    }
}

public class IndexPartyOnUpdatedHandler : INotificationHandler<PartyUpdatedEvent>
{
    private readonly ISearchService _search;

    public IndexPartyOnUpdatedHandler(ISearchService search)
    {
        _search = search;
    }

    public async Task Handle(PartyUpdatedEvent notification, CancellationToken ct)
    {
        await _search.IndexPartyAsync(new PartySearchDocument(
            notification.PartyId.ToString(),
            notification.Name,
            null, null, null, [], true
        ));
    }
}
