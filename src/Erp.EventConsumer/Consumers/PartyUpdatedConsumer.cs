using Dapper;
using Erp.Domain.Parties.Events;
using Erp.Domain.Search;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Erp.EventConsumer.Consumers;

file record PartyIndexRow(string Name, string? City, string? Email, string? Phone, bool IsCustomer, bool IsSupplier);

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

        try
        {
            await conn.ExecuteAsync(@"
                INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at, message_id)
                VALUES (@AggregateId, @AggregateType, @EventType, @Payload, @OccurredAt, @MessageId)",
                new
                {
                    AggregateId = e.PartyId,
                    AggregateType = "Party",
                    EventType = "PartyUpdated",
                    Payload = JsonSerializer.Serialize(e),
                    OccurredAt = e.OccurredAt,
                    context.MessageId
                });
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return;
        }

        var row = await conn.QuerySingleOrDefaultAsync<PartyIndexRow>(@"
            SELECT p.name,
                   a.city,
                   em.value  AS email,
                   ph.value  AS phone,
                   CAST(CASE WHEN cr.party_id IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS is_customer,
                   CAST(CASE WHEN sr.party_id IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS is_supplier
            FROM mdata.parties p
            LEFT JOIN mdata.party_addresses a
                   ON a.party_id = p.id AND a.is_default = 1 AND a.address_type_id = 1
            LEFT JOIN mdata.party_contact_methods em
                   ON em.party_id = p.id AND em.contact_method_type_id = 2 AND em.is_primary = 1
            LEFT JOIN mdata.party_contact_methods ph
                   ON ph.party_id = p.id AND ph.contact_method_type_id = 1 AND ph.is_primary = 1
            LEFT JOIN mdata.customer_roles cr ON cr.party_id = p.id
            LEFT JOIN mdata.supplier_roles sr ON sr.party_id = p.id
            WHERE p.id = @PartyId",
            new { e.PartyId });

        if (row is not null)
        {
            var roles = new List<string>();
            if (row.IsCustomer) roles.Add("customer");
            if (row.IsSupplier) roles.Add("supplier");

            await _search.IndexPartyAsync(new PartySearchDocument(
                e.PartyId.ToString(), row.Name, row.City, row.Email, row.Phone, [.. roles], true));
        }

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
