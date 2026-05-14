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

}
