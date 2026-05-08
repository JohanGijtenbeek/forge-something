using Erp.Seeder.Models;
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

    public async Task WriteAsync(List<GeneratedParty> parties, CancellationToken ct = default)
    {
        Console.WriteLine($"  → {parties.Count} parties naar Meilisearch...");

        // Zorg dat de index bestaat
        try
        {
            await _client.CreateIndexAsync(PartiesIndex, "id");
            await Task.Delay(300, ct);
        }
        catch
        {
            /* index bestaat al */
        }

        var index = _client.Index(PartiesIndex);
        await index.UpdateSearchableAttributesAsync(["name", "city", "email", "phone"]);

        // Meilisearch indexeert het best in batches van 1000
        var documents = parties
            .Where(p => p.Party.IsActive)
            .Select(p =>
            {
                var roles = new List<string>();
                if (p.CustomerRole != null) roles.Add("customer");
                if (p.SupplierRole != null) roles.Add("supplier");

                var city = p.Addresses.FirstOrDefault(a => a.IsDefault && a.AddressTypeId == 1)?.City;
                var email = p.ContactMethods.FirstOrDefault(c => c.ContactMethodTypeId == 2)?.Value;
                var phone = p.ContactMethods.FirstOrDefault(c => c.ContactMethodTypeId == 1)?.Value;

                return new
                {
                    id = p.Party.Id.ToString(),
                    name = p.Party.Name,
                    city,
                    email,
                    phone,
                    roles = roles.ToArray(),
                    isActive = p.Party.IsActive,
                    entityType = "party",
                    displayLabel = p.Party.Name
                };
            })
            .ToList();

        const int batchSize = 1000;
        var batches = documents.Chunk(batchSize).ToList();

        for (var i = 0; i < batches.Count; i++)
        {
            var task = await index.AddDocumentsAsync(batches[i].ToList());
            Console.WriteLine($"\r    Batch {i + 1}/{batches.Count} geïndexeerd (taskUid={task.TaskUid})");
        }

        Console.WriteLine("  ✓ Meilisearch klaar");
    }
}
