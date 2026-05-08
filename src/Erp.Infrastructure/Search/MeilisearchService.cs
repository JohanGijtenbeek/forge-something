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

            results.AddRange(hits.Hits.Select(p => new SearchResult(
                p.Id,
                p.EntityType,
                p.DisplayLabel,
                p.City
            )));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zoeken mislukt in '{Index}'", PartiesIndex);
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
}
