using Erp.Seeder.Generators;
using Erp.Seeder.Models;
using System.Collections.Concurrent;
using System.Net.Http.Json;

namespace Erp.Seeder.Writers;

public class ApiWriter
{
    private readonly HttpClient _client;
    private readonly PartyGenerator _generator;
    private readonly int _parallelism;

    public ApiWriter(string apiUrl, PartyGenerator generator, int parallelism = 5)
    {
        _client = new HttpClient { BaseAddress = new Uri(apiUrl) };
        _generator = generator;
        _parallelism = parallelism;
    }

    public async Task<(int parties, int relationships, int errors)> WriteAsync(
        List<GeneratedParty> organizations,
        List<GeneratedParty> persons,
        CancellationToken ct = default)
    {
        var orgIds = new ConcurrentBag<Guid>();
        var personIds = new ConcurrentBag<Guid>();
        var errors = 0;
        var completed = 0;
        var total = organizations.Count + persons.Count;

        Console.WriteLine($"  → {organizations.Count} organisaties via API...");

        await Parallel.ForEachAsync(organizations, new ParallelOptions
        {
            MaxDegreeOfParallelism = _parallelism,
            CancellationToken = ct
        }, async (party, partyCt) =>
        {
            var id = await PostOrganizationAsync(party, partyCt);
            if (id.HasValue)
            {
                orgIds.Add(id.Value);
                var done = Interlocked.Increment(ref completed);
                Console.Write($"\r  {done}/{total} parties aangemaakt...");
            }
            else
            {
                Interlocked.Increment(ref errors);
            }
        });

        Console.WriteLine($"\r  {completed}/{total} parties aangemaakt...");
        Console.WriteLine($"  → {persons.Count} personen via API...");

        await Parallel.ForEachAsync(persons, new ParallelOptions
        {
            MaxDegreeOfParallelism = _parallelism,
            CancellationToken = ct
        }, async (party, partyCt) =>
        {
            var id = await PostPersonAsync(party, partyCt);
            if (id.HasValue)
            {
                personIds.Add(id.Value);
                var done = Interlocked.Increment(ref completed);
                Console.Write($"\r  {done}/{total} parties aangemaakt...");
            }
            else
            {
                Interlocked.Increment(ref errors);
            }
        });

        Console.WriteLine();

        var orgIdList = orgIds.ToList();
        var personIdList = personIds.ToList();
        var relationships = _generator.GenerateRelationships(orgIdList, personIdList);

        Console.WriteLine($"  → {relationships.Count} relaties via API...");

        var relErrors = 0;
        var relCompleted = 0;

        await Parallel.ForEachAsync(relationships, new ParallelOptions
        {
            MaxDegreeOfParallelism = _parallelism,
            CancellationToken = ct
        }, async (rel, partyCt) =>
        {
            var ok = await PostRelationshipAsync(rel, partyCt);
            if (ok)
                Interlocked.Increment(ref relCompleted);
            else
                Interlocked.Increment(ref relErrors);
        });

        if (relErrors > 0)
            Console.WriteLine($"  ⚠ {relCompleted} relaties aangemaakt, {relErrors} fouten");
        else
            Console.WriteLine($"  ✓ {relCompleted} relaties aangemaakt");

        return (completed, relCompleted, errors + relErrors);
    }

    private async Task<Guid?> PostOrganizationAsync(GeneratedParty party, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/parties/organizations", new
            {
                name = party.Party.Name,
                vatNumber = party.OrganizationDetail?.VatNumber,
                chamberOfCommerceNumber = party.OrganizationDetail?.ChamberOfCommerceNumber,
                registerAsCustomer = party.CustomerRole != null,
                registerAsSupplier = party.SupplierRole != null
            }, ct);

            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<IdResponse>(ct);
            return result?.Id;
        }
        catch
        {
            return null;
        }
    }

    private async Task<Guid?> PostPersonAsync(GeneratedParty party, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/parties/persons", new
            {
                firstName = party.PersonDetail!.FirstName,
                middleName = party.PersonDetail.MiddleName,
                lastName = party.PersonDetail.LastName,
                initials = party.PersonDetail.Initials
            }, ct);

            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<IdResponse>(ct);
            return result?.Id;
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> PostRelationshipAsync(PartyRelationshipRow rel, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(
                $"/api/parties/{rel.FromPartyId}/relationships",
                new { toPartyId = rel.ToPartyId, relationshipTypeId = rel.RelationshipTypeId },
                ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(int articles, int errors)> WriteArticlesAsync(
        List<ArticleSeedRow> articles,
        CancellationToken ct = default)
    {
        var errors = 0;
        var completed = 0;

        Console.WriteLine($"  → {articles.Count} artikelen via API...");

        await Parallel.ForEachAsync(articles, new ParallelOptions
        {
            MaxDegreeOfParallelism = _parallelism,
            CancellationToken = ct
        }, async (article, articleCt) =>
        {
            var ok = await PostArticleAsync(article, articleCt);
            if (ok)
            {
                var done = Interlocked.Increment(ref completed);
                Console.Write($"\r  {done}/{articles.Count} artikelen aangemaakt...");
            }
            else
            {
                Interlocked.Increment(ref errors);
            }
        });

        Console.WriteLine();
        return (completed, errors);
    }

    private async Task<bool> PostArticleAsync(ArticleSeedRow article, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/articles", new
            {
                code = article.Code,
                name = article.Name,
                articleType = article.ArticleType,
                description = article.Description,
                categoryId = article.CategoryId,
                unitOfMeasureId = article.UnitOfMeasureId,
                purchasePrice = article.PurchasePrice
            }, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(int orders, int errors)> WriteOrdersAsync(
        List<Erp.Seeder.Models.OrderSeedRow> orders,
        CancellationToken ct = default)
    {
        var errors = 0;
        var completed = 0;

        Console.WriteLine($"  → {orders.Count} orders via API...");

        await Parallel.ForEachAsync(orders, new ParallelOptions
        {
            MaxDegreeOfParallelism = _parallelism,
            CancellationToken = ct
        }, async (order, orderCt) =>
        {
            var ok = await PostOrderAsync(order, orderCt);
            if (ok)
            {
                var done = Interlocked.Increment(ref completed);
                Console.Write($"\r  {done}/{orders.Count} orders aangemaakt...");
            }
            else
            {
                Interlocked.Increment(ref errors);
            }
        });

        Console.WriteLine();
        return (completed, errors);
    }

    private int _orderErrorsLogged;

    private async Task<bool> PostOrderAsync(Erp.Seeder.Models.OrderSeedRow order, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/orders", new
            {
                articleId = order.ArticleId,
                customerId = order.CustomerId,
                quantity = order.Quantity,
                unitOfMeasure = order.UnitOfMeasure,
                dueDate = order.DueDate,
                notes = order.Notes
            }, ct);

            if (response.IsSuccessStatusCode) return true;

            if (Interlocked.Increment(ref _orderErrorsLogged) <= 3)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                Console.WriteLine($"\n  ✗ Order error {(int)response.StatusCode}: {body[..Math.Min(200, body.Length)]}");
            }
            return false;
        }
        catch (Exception ex)
        {
            if (Interlocked.Increment(ref _orderErrorsLogged) <= 3)
                Console.WriteLine($"\n  ✗ Order exception: {ex.Message}");
            return false;
        }
    }

    private record IdResponse(Guid Id);
}
