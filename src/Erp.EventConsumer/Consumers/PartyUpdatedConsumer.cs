using Dapper;
using Erp.Domain.Parties.Events;
using Erp.Domain.Search;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Erp.EventConsumer.Consumers;

public class PartyUpdatedConsumer : IConsumer<PartyUpdatedEvent>
{
    private readonly DbConnectionFactory _factory;
    private readonly ISearchService _search;
    private readonly IHubContext<EventHub> _hub;

    public PartyUpdatedConsumer(DbConnectionFactory factory, ISearchService search, IHubContext<EventHub> hub)
    {
        _factory = factory;
        _search = search;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<PartyUpdatedEvent> context)
    {
        var e = context.Message;
        var ct = context.CancellationToken;

        using var conn = _factory.Create();

        await conn.ExecuteAsync(@"
            INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at)
            VALUES (@AggregateId, @AggregateType, @EventType, @Payload, @OccurredAt)",
            new
            {
                AggregateId = e.PartyId,
                AggregateType = "Party",
                EventType = "PartyUpdated",
                Payload = JsonSerializer.Serialize(e),
                OccurredAt = e.OccurredAt
            });

        await _search.IndexPartyAsync(new PartySearchDocument(
            e.PartyId.ToString(), e.Name, null, null, null, [], true));

        await _hub.Clients.All.SendAsync("EventReceived", new
        {
            eventType = "PartyUpdated",
            aggregateType = "Party",
            aggregateId = e.PartyId.ToString(),
            occurredAt = e.OccurredAt,
            payload = (object)e
        }, ct);
    }
}
