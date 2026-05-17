using Dapper;
using Erp.Domain.Quotes.Events;
using Erp.Domain.Search;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Erp.EventConsumer.Consumers;

public class QuoteStatusChangedConsumer : IConsumer<QuoteStatusChangedEvent>
{
    private readonly DbConnectionFactory _factory;
    private readonly ISearchService _search;
    private readonly IHubContext<EventHub> _hub;

    public QuoteStatusChangedConsumer(DbConnectionFactory factory, ISearchService search, IHubContext<EventHub> hub)
    {
        _factory = factory;
        _search = search;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<QuoteStatusChangedEvent> context)
    {
        var e = context.Message;
        var ct = context.CancellationToken;

        using var conn = _factory.Create();
        await conn.OpenAsync(ct);

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
                        AggregateId   = e.QuoteId,
                        AggregateType = "Quote",
                        EventType     = "QuoteStatusChanged",
                        Payload       = JsonSerializer.Serialize(e),
                        OccurredAt    = e.OccurredAt,
                        context.MessageId
                    }, tx);

                await conn.ExecuteAsync(@"
                    INSERT INTO audit.quote_history
                        (quote_id, event_sequence, event_type, summary, changed_by, changed_at, snapshot)
                    VALUES
                        (@QuoteId, @EventSequence, @EventType, @Summary, @ChangedBy, @ChangedAt, @Snapshot)",
                    new
                    {
                        e.QuoteId,
                        EventSequence = eventId,
                        EventType     = "QuoteStatusChanged",
                        Summary       = $"Quote #{e.QuoteNumber} status: {e.OldStatus} → {e.NewStatus}",
                        ChangedBy     = "system",
                        ChangedAt     = e.OccurredAt,
                        Snapshot      = JsonSerializer.Serialize(e)
                    }, tx);

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

        await _hub.Clients.All.SendAsync("EventReceived", new
        {
            eventType     = "QuoteStatusChanged",
            aggregateType = "Quote",
            aggregateId   = e.QuoteId.ToString(),
            occurredAt    = e.OccurredAt,
            payload       = (object)e
        }, ct);
    }
}
