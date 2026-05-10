using Dapper;
using Erp.Domain.Articles.Events;
using Erp.Domain.Search;
using Erp.EventConsumer.Hubs;
using Erp.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Erp.EventConsumer.Consumers;

file record ArticleIndexRow(string Code, string Name, string? CategoryName);

public class ArticleUpdatedConsumer : IConsumer<ArticleUpdatedEvent>
{
    private readonly DbConnectionFactory _factory;
    private readonly ISearchService _search;
    private readonly IHubContext<EventHub> _hub;

    public ArticleUpdatedConsumer(DbConnectionFactory factory, ISearchService search, IHubContext<EventHub> hub)
    {
        _factory = factory;
        _search = search;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<ArticleUpdatedEvent> context)
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
                    AggregateId = e.ArticleId,
                    AggregateType = "Article",
                    EventType = "ArticleUpdated",
                    Payload = JsonSerializer.Serialize(e),
                    OccurredAt = e.OccurredAt,
                    context.MessageId
                });
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return;
        }

        var row = await conn.QuerySingleOrDefaultAsync<ArticleIndexRow>(@"
            SELECT a.code, a.name, c.name AS category_name
            FROM mdata.articles a
            LEFT JOIN mdata.article_categories c ON c.id = a.category_id
            WHERE a.id = @ArticleId",
            new { e.ArticleId });

        if (row is not null)
        {
            await _search.IndexArticleAsync(new ArticleSearchDocument(
                e.ArticleId.ToString(), row.Code, row.Name, row.CategoryName, true));
        }

        await _hub.Clients.All.SendAsync("EventReceived", new
        {
            eventType = "ArticleUpdated",
            aggregateType = "Article",
            aggregateId = e.ArticleId.ToString(),
            occurredAt = e.OccurredAt,
            payload = (object)e
        }, ct);
    }
}
