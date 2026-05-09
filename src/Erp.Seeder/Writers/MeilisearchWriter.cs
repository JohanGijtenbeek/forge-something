using Meilisearch;

namespace Erp.Seeder.Writers;

public class MeilisearchWriter
{
    private readonly MeilisearchClient _client;
    private const string PartiesIndex = "parties";

    public MeilisearchWriter(string url, string? apiKey)
    {
        _client = string.IsNullOrEmpty(apiKey)
            ? new MeilisearchClient(url)
            : new MeilisearchClient(url, apiKey);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        Console.WriteLine("  → Meilisearch leegmaken...");
        try
        {
            await _client.DeleteIndexAsync(PartiesIndex);
            await Task.Delay(300, ct);
            Console.WriteLine("  ✓ Meilisearch leeg");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✓ Meilisearch index bestond niet ({ex.Message})");
        }
    }
}
