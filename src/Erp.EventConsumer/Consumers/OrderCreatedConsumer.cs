using Dapper;
using Erp.Domain.Orders.Events;
using Erp.Domain.Search;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Erp.EventConsumer.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly DbConnectionFactory _factory;
    private readonly ISearchService _search;
    private readonly IHubContext<EventHub> _hub;

    public OrderCreatedConsumer(DbConnectionFactory factory, ISearchService search, IHubContext<EventHub> hub)
    {
        _factory = factory;
        _search = search;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
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
                        AggregateId = e.OrderId,
                        AggregateType = "Order",
                        EventType = "OrderCreated",
                        Payload = JsonSerializer.Serialize(e),
                        OccurredAt = e.OccurredAt,
                        context.MessageId
                    }, tx);

                var snapshot = JsonSerializer.Serialize(e);

                await conn.ExecuteAsync(@"
                    INSERT INTO audit.order_history
                        (order_id, event_sequence, event_type, summary, changed_by, changed_at, snapshot)
                    VALUES
                        (@OrderId, @EventSequence, @EventType, @Summary, @ChangedBy, @ChangedAt, @Snapshot)",
                    new
                    {
                        e.OrderId,
                        EventSequence = eventId,
                        EventType = "OrderCreated",
                        Summary = $"Order #{e.OrderNumber} created: {e.ArticleCode} — {e.ArticleName}",
                        ChangedBy = "system",
                        ChangedAt = e.OccurredAt,
                        Snapshot = snapshot
                    }, tx);

                await conn.ExecuteAsync(@"
                    INSERT INTO audit.order_snapshots (order_id, at_event_id, snapshot, trigger_reason)
                    VALUES (@OrderId, @AtEventId, @Snapshot, @TriggerReason)",
                    new { e.OrderId, AtEventId = eventId, Snapshot = snapshot, TriggerReason = "state_created" }, tx);

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

        await _search.IndexOrderAsync(new OrderSearchDocument(
            e.OrderId.ToString(), e.OrderNumber, e.ArticleCode, e.ArticleName,
            e.CustomerName, "draft"));

        await _hub.Clients.All.SendAsync("EventReceived", new
        {
            eventType = "OrderCreated",
            aggregateType = "Order",
            aggregateId = e.OrderId.ToString(),
            occurredAt = e.OccurredAt,
            payload = (object)e
        }, ct);
    }
}
