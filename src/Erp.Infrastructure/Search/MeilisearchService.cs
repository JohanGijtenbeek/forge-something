using Erp.Domain.Parties;
using Erp.Domain.Search;
using Meilisearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Erp.Infrastructure.Search;

public class MeilisearchService : ISearchService
{
    private readonly MeilisearchClient _client;
    private readonly ILogger<MeilisearchService> _logger;

    private const string PartiesIndex = "parties";
    private const string ArticlesIndex = "articles";
    private const string OrdersIndex = "orders";
    private const string QuotesIndex = "quotes";

    public MeilisearchService(IConfiguration config, ILogger<MeilisearchService> logger)
    {
        var url = config["Meilisearch:Url"] ?? "http://localhost:7700";
        var apiKey = config["Meilisearch:ApiKey"];
        _client = string.IsNullOrEmpty(apiKey)
            ? new MeilisearchClient(url)
            : new MeilisearchClient(url, apiKey);
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await EnsureIndexAsync(PartiesIndex, "id", ["name", "city", "email", "phone"]);
        await EnsureIndexAsync(ArticlesIndex, "id", ["code", "name", "category"]);
        await EnsureIndexAsync(OrdersIndex, "id", ["orderNumber", "articleCode", "articleName", "customerName"]);
        await EnsureIndexAsync(QuotesIndex, "id", ["quoteNumber", "customerName"]);
    }

    public async Task<int> ReindexPartiesAsync(IPartyRepository repository, CancellationToken ct = default)
    {
        var parties = await repository.GetAllAsync(includeInactive: false, ct);

        var documents = parties.Select(party =>
        {
            var roles = new List<string>();
            if (party.IsCustomer) roles.Add("customer");
            if (party.IsSupplier) roles.Add("supplier");

            return new PartySearchDocument(
                party.Id.ToString(),
                party.Name,
                party.Addresses.FirstOrDefault(a => a.IsDefault)?.City,
                party.ContactMethods.FirstOrDefault(c => c.ContactMethodType == ContactMethodType.Email)?.Value,
                party.ContactMethods.FirstOrDefault(c => c.ContactMethodType == ContactMethodType.Phone)?.Value,
                [.. roles],
                party.IsActive
            );
        }).ToList();

        if (documents.Count == 0)
        {
            _logger.LogInformation("Geen parties om te indexeren");
            return 0;
        }

        try
        {
            var index = _client.Index(PartiesIndex);
            var task = await index.AddDocumentsAsync(documents);
            _logger.LogInformation("Meilisearch geseed met {Count} parties (taskUid={Uid})",
                documents.Count, task.TaskUid);
            return documents.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reindex mislukt");
            return 0;
        }
    }

    private async Task EnsureIndexAsync(string indexName, string primaryKey, string[] searchableAttributes)
    {
        try
        {
            var task = await _client.CreateIndexAsync(indexName, primaryKey);
            await Task.Delay(300);
            _logger.LogInformation("Index '{Index}' aangemaakt", indexName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Index '{Index}' aanmaken: {Msg} (bestaat mogelijk al)", indexName, ex.Message);
        }

        var index = _client.Index(indexName);
        await index.UpdateSearchableAttributesAsync(searchableAttributes);
    }

    public async Task<IEnumerable<SearchResult>> GlobalSearchAsync(string query, int limit = 5)
    {
        var results = new List<SearchResult>();

        try
        {
            var index = _client.Index(PartiesIndex);
            var hits = await index.SearchAsync<PartySearchDocument>(query, new SearchQuery { Limit = limit });
            results.AddRange(hits.Hits.Select(p => new SearchResult(p.Id, p.EntityType, p.DisplayLabel, p.City)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zoeken mislukt in '{Index}'", PartiesIndex);
        }

        try
        {
            var index = _client.Index(ArticlesIndex);
            var hits = await index.SearchAsync<ArticleSearchDocument>(query, new SearchQuery { Limit = limit });
            results.AddRange(hits.Hits.Select(a => new SearchResult(a.Id, a.EntityType, a.DisplayLabel, a.Category)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zoeken mislukt in '{Index}'", ArticlesIndex);
        }

        try
        {
            var index = _client.Index(OrdersIndex);
            var hits = await index.SearchAsync<OrderSearchDocument>(query, new SearchQuery { Limit = limit });
            results.AddRange(hits.Hits.Select(o => new SearchResult(o.Id, o.EntityType, o.DisplayLabel, o.CustomerName)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zoeken mislukt in '{Index}'", OrdersIndex);
        }

        try
        {
            var index = _client.Index(QuotesIndex);
            var hits = await index.SearchAsync<QuoteSearchDocument>(query, new SearchQuery { Limit = limit });
            results.AddRange(hits.Hits.Select(q => new SearchResult(q.Id, q.EntityType, q.DisplayLabel, q.Status)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zoeken mislukt in '{Index}'", QuotesIndex);
        }

        return results;
    }

    public async Task IndexPartyAsync(PartySearchDocument document)
    {
        try
        {
            var index = _client.Index(PartiesIndex);
            var task = await index.AddDocumentsAsync(new[] { document });
            _logger.LogInformation("Party '{Id}' geïndexeerd (taskUid={Uid})", document.Id, task.TaskUid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexeren mislukt voor party '{Id}'", document.Id);
        }
    }

    public async Task DeletePartyAsync(string id)
    {
        try
        {
            var index = _client.Index(PartiesIndex);
            await index.DeleteOneDocumentAsync(id);
            _logger.LogInformation("Party '{Id}' verwijderd uit index", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verwijderen mislukt voor party '{Id}'", id);
        }
    }

    public async Task IndexArticleAsync(ArticleSearchDocument document)
    {
        try
        {
            var index = _client.Index(ArticlesIndex);
            var task = await index.AddDocumentsAsync(new[] { document });
            _logger.LogInformation("Article '{Id}' geïndexeerd (taskUid={Uid})", document.Id, task.TaskUid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexeren mislukt voor article '{Id}'", document.Id);
        }
    }

    public async Task DeleteArticleAsync(string id)
    {
        try
        {
            var index = _client.Index(ArticlesIndex);
            await index.DeleteOneDocumentAsync(id);
            _logger.LogInformation("Article '{Id}' verwijderd uit index", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verwijderen mislukt voor article '{Id}'", id);
        }
    }

    public async Task IndexOrderAsync(OrderSearchDocument document)
    {
        try
        {
            var index = _client.Index(OrdersIndex);
            var task = await index.AddDocumentsAsync(new[] { document });
            _logger.LogInformation("Order '{Id}' geïndexeerd (taskUid={Uid})", document.Id, task.TaskUid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexeren mislukt voor order '{Id}'", document.Id);
        }
    }

    public async Task DeleteOrderAsync(string id)
    {
        try
        {
            var index = _client.Index(OrdersIndex);
            await index.DeleteOneDocumentAsync(id);
            _logger.LogInformation("Order '{Id}' verwijderd uit index", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verwijderen mislukt voor order '{Id}'", id);
        }
    }

    public async Task IndexQuoteAsync(QuoteSearchDocument document)
    {
        try
        {
            var index = _client.Index(QuotesIndex);
            var task = await index.AddDocumentsAsync(new[] { document });
            _logger.LogInformation("Quote '{Id}' geïndexeerd (taskUid={Uid})", document.Id, task.TaskUid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Indexeren mislukt voor quote '{Id}'", document.Id);
        }
    }

    public async Task DeleteQuoteAsync(string id)
    {
        try
        {
            var index = _client.Index(QuotesIndex);
            await index.DeleteOneDocumentAsync(id);
            _logger.LogInformation("Quote '{Id}' verwijderd uit index", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verwijderen mislukt voor quote '{Id}'", id);
        }
    }

    public async Task<IEnumerable<SearchResult>> GlobalSearchQuotesAsync(string query, int limit)
    {
        try
        {
            var index = _client.Index(QuotesIndex);
            var hits = await index.SearchAsync<QuoteSearchDocument>(query, new SearchQuery { Limit = limit });
            return hits.Hits.Select(q => new SearchResult(q.Id, q.EntityType, q.DisplayLabel, q.Status));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zoeken mislukt in '{Index}'", QuotesIndex);
            return [];
        }
    }
}
