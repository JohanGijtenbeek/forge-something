using Erp.Seeder.Models;
using Meilisearch;

namespace Erp.Seeder.Writers;

public class MeilisearchWriter
{
    private readonly MeilisearchClient _client;
    private const string PartiesIndex = "parties";
    private const string ArticlesIndex = "articles";

    public MeilisearchWriter(string url, string? apiKey)
    {
        _client = string.IsNullOrEmpty(apiKey)
            ? new MeilisearchClient(url)
            : new MeilisearchClient(url, apiKey);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        Console.WriteLine("  → Meilisearch leegmaken...");
        foreach (var indexName in new[] { PartiesIndex, ArticlesIndex })
        {
            try
            {
                await _client.DeleteIndexAsync(indexName);
                await Task.Delay(200, ct);
            }
            catch
            {
                /* index bestond niet */
            }
        }
        Console.WriteLine("  ✓ Meilisearch leeg");
    }

    public async Task WriteArticlesAsync(List<ArticleSeedRow> articles, Dictionary<Guid, string> categoryNames,
        CancellationToken ct = default)
    {
        Console.WriteLine($"  → {articles.Count} artikelen naar Meilisearch...");

        try
        {
            await _client.CreateIndexAsync(ArticlesIndex, "id");
            await Task.Delay(300, ct);
        }
        catch { /* index bestaat al */ }

        var index = _client.Index(ArticlesIndex);
        await index.UpdateSearchableAttributesAsync(["code", "name", "category"]);

        var documents = articles
            .Where(a => a.IsActive)
            .Select(a => new
            {
                id = a.Id.ToString(),
                code = a.Code,
                name = a.Name,
                category = a.CategoryId.HasValue && categoryNames.TryGetValue(a.CategoryId.Value, out var cat) ? cat : null,
                isActive = a.IsActive,
                entityType = "article",
                displayLabel = $"{a.Code} - {a.Name}"
            })
            .ToList();

        if (documents.Count > 0)
        {
            var task = await index.AddDocumentsAsync(documents);
            Console.WriteLine($"    Geïndexeerd (taskUid={task.TaskUid})");
        }

        Console.WriteLine("  ✓ Artikelen Meilisearch klaar");
    }
}
