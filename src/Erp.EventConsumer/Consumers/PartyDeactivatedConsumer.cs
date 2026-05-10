using Dapper;
using Erp.Domain.Parties.Events;
using Erp.Domain.Search;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Erp.EventConsumer.Consumers;

public class PartyDeactivatedConsumer : IConsumer<PartyDeactivatedEvent>
{
    private readonly DbConnectionFactory _factory;
    private readonly ISearchService _search;
    private readonly IHubContext<EventHub> _hub;

    public PartyDeactivatedConsumer(DbConnectionFactory factory, ISearchService search, IHubContext<EventHub> hub)
    {
        _factory = factory;
        _search = search;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<PartyDeactivatedEvent> context)
    {
        var e = context.Message;
        var ct = context.CancellationToken;

        using var conn = _factory.Create();

        using (var tx = await conn.BeginTransactionAsync(ct))
        {
            try
            {
                var eventId = await conn.QuerySingleAsync<long>(@"
                    INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at, message_id)
                    OUTPUT INSERTED.id
                    VALUES (@AggregateId, @AggregateType, @EventType, @Payload, @OccurredAt, @MessageId)",
                    new
                    {
                        AggregateId = e.PartyId,
                        AggregateType = "Party",
                        EventType = "PartyDeactivated",
                        Payload = JsonSerializer.Serialize(e),
                        OccurredAt = e.OccurredAt,
                        context.MessageId
                    }, tx);

                var snapshot = JsonSerializer.Serialize(e);

                await conn.ExecuteAsync(@"
                    INSERT INTO audit.party_history
                        (party_id, event_sequence, event_type, summary, changed_by, changed_at, snapshot)
                    VALUES
                        (@PartyId, @EventSequence, @EventType, @Summary, @ChangedBy, @ChangedAt, @Snapshot)",
                    new
                    {
                        e.PartyId,
                        EventSequence = eventId,
                        EventType = "PartyDeactivated",
                        Summary = $"Party gedeactiveerd: {e.Name}",
                        ChangedBy = "system",
                        ChangedAt = e.OccurredAt,
                        Snapshot = snapshot
                    }, tx);

                await conn.ExecuteAsync(@"
                    INSERT INTO audit.party_snapshots (party_id, at_event_id, snapshot, trigger_reason)
                    VALUES (@PartyId, @AtEventId, @Snapshot, @TriggerReason)",
                    new { PartyId = e.PartyId, AtEventId = eventId, Snapshot = snapshot, TriggerReason = "state_closed" }, tx);

                await tx.CommitAsync(ct);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
            {
                return;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        await _search.DeletePartyAsync(e.PartyId.ToString());

        await _hub.Clients.All.SendAsync("EventReceived", new
        {
            eventType = "PartyDeactivated",
            aggregateType = "Party",
            aggregateId = e.PartyId.ToString(),
            occurredAt = e.OccurredAt,
            payload = (object)e
        }, ct);
    }
}
