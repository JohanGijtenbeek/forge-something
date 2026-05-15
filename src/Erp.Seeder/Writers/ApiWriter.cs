using Bogus;
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
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(int done, int errors)> WriteArticleBomAndOperationsAsync(int seed, CancellationToken ct = default)
    {
        var faker = new Faker { Random = new Randomizer(seed + 400) };

        var manufacturedResponse = await _client.GetFromJsonAsync<ArticlePagedResult>(
            "/api/articles?articleType=manufactured&pageSize=500&page=1", ct);
        var manufactured = manufacturedResponse?.Items ?? [];

        var rawResponse = await _client.GetFromJsonAsync<ArticlePagedResult>(
            "/api/articles?articleType=raw_material&pageSize=500&page=1", ct);
        var rawMaterials = rawResponse?.Items ?? [];

        var operationTypes = await _client.GetFromJsonAsync<List<OperationTypeItem>>(
            "/api/operation-types", ct) ?? [];

        if (manufactured.Count == 0 || operationTypes.Count == 0)
        {
            Console.WriteLine("  ⚠ Geen gefabriceerde artikelen of bewerkingstypes — stap overgeslagen.");
            return (0, 0);
        }

        Console.WriteLine($"  → BOM en routing voor {manufactured.Count} gefabriceerde artikelen...");

        var done = 0;
        var errors = 0;

        foreach (var article in manufactured)
        {
            var componentCount = faker.Random.Int(2, 4);
            var usedComponents = new HashSet<Guid>();

            for (var i = 0; i < componentCount && rawMaterials.Count > 0; i++)
            {
                ArticleItem component;
                var attempts = 0;
                do { component = faker.PickRandom(rawMaterials); }
                while (usedComponents.Contains(component.Id) && ++attempts < 10);
                if (usedComponents.Contains(component.Id)) continue;

                usedComponents.Add(component.Id);
                var ok = await PostBomComponentAsync(
                    article.Id, component.Id,
                    Math.Round(faker.Random.Decimal(0.5m, 20m), 4),
                    (i + 1) * 10, ct);
                if (!ok) errors++;
            }

            var opCount = faker.Random.Int(2, 5);
            for (var i = 0; i < opCount; i++)
            {
                var opType = faker.PickRandom(operationTypes);
                var ok = await PostArticleOperationAsync(
                    article.Id,
                    (i + 1) * 10,
                    opType.Id,
                    Math.Round(faker.Random.Decimal(15m, 120m), 2),
                    faker.Random.Bool(0.1f), ct);
                if (!ok) errors++;
            }

            done++;
            Console.Write($"\r  {done}/{manufactured.Count} artikelen verrijkt...");
        }

        Console.WriteLine();
        return (done, errors);
    }

    private async Task<bool> PostBomComponentAsync(
        Guid articleId, Guid childArticleId, decimal quantity, int sortOrder, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync($"/api/articles/{articleId}/bom", new
            {
                childArticleId,
                quantity,
                unitOfMeasureId = (Guid?)null,
                sortOrder
            }, ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    private async Task<bool> PostArticleOperationAsync(
        Guid articleId, int sequenceNumber, Guid operationTypeId,
        decimal estimatedMinutes, bool isConditional, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync($"/api/articles/{articleId}/operations", new
            {
                sequenceNumber,
                operationTypeId,
                estimatedMinutes,
                notes = (string?)null,
                isConditional
            }, ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<(int quotes, int errors)> WriteQuotesAsync(
        List<Erp.Seeder.Models.QuoteSeedRow> quotes,
        CancellationToken ct = default)
    {
        var errors = 0;
        var completed = 0;

        Console.WriteLine($"  → {quotes.Count} offertes via API...");

        // Sequential: each quote needs its lines posted after the header
        foreach (var quote in quotes)
        {
            var quoteId = await PostQuoteAsync(quote, ct);
            if (quoteId is null)
            {
                errors++;
                continue;
            }

            var lineOk = true;
            var lineIds = new List<Guid>();
            foreach (var line in quote.Lines)
            {
                var lineId = await PostQuoteLineAsync(quoteId.Value, line, ct);
                if (lineId is null) { lineOk = false; break; }
                if (line.ShouldAccept) lineIds.Add(lineId.Value);
            }

            if (!lineOk) { errors++; continue; }

            foreach (var lineId in lineIds)
                await AcceptQuoteLineAsync(quoteId.Value, lineId, ct);

            if (quote.TargetStatus != "draft")
                await UpdateQuoteStatusAsync(quoteId.Value, quote.TargetStatus, ct);

            completed++;
            Console.Write($"\r  {completed}/{quotes.Count} offertes aangemaakt...");
        }

        Console.WriteLine();
        return (completed, errors);
    }

    private async Task<Guid?> PostQuoteAsync(Erp.Seeder.Models.QuoteSeedRow quote, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/quotes", new
            {
                customerId     = quote.CustomerId,
                date           = quote.Date,
                reference      = quote.Reference,
                contactPerson  = quote.ContactPerson,
                deliveryTime   = quote.DeliveryTime,
                hourlyRate     = quote.HourlyRate,
                materialMargin = quote.MaterialMargin,
                standardMargin = quote.StandardMargin,
                setupTime      = quote.SetupTime,
            }, ct);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<IdResponse>(ct);
            return result?.Id;
        }
        catch { return null; }
    }

    private async Task<Guid?> PostQuoteLineAsync(Guid quoteId, Erp.Seeder.Models.QuoteLineSeedRow line, CancellationToken ct)
    {
        try
        {
            var response = await _client.PostAsJsonAsync($"/api/quotes/{quoteId}/lines", new
            {
                sortOrder            = line.SortOrder,
                partName             = line.PartName,
                partNumber           = line.PartNumber,
                quantity             = line.Quantity,
                articleId            = (Guid?)null,
                materialType         = line.MaterialType,
                materialCode         = line.MaterialCode,
                materialCode2        = line.MaterialCode2,
                materialGeometry     = line.MaterialGeometry,
                materialSizeMm       = line.MaterialSizeMm,
                materialLengthMm     = line.MaterialLengthMm,
                materialQuantity     = line.MaterialQuantity,
                materialPrice        = line.MaterialPrice,
                materialSource       = line.MaterialSource,
                operationCount       = line.OperationCount,
                operationTimeMinutes = line.OperationTimeMinutes,
                subcontractingCount  = line.SubcontractingCount,
                subcontractingPrice  = line.SubcontractingPrice,
                isManualPrice        = line.IsManualPrice,
                manualPrice          = line.ManualPrice,
                remarks              = (string?)null,
            }, ct);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<IdResponse>(ct);
            return result?.Id;
        }
        catch { return null; }
    }

    private async Task AcceptQuoteLineAsync(Guid quoteId, Guid lineId, CancellationToken ct)
    {
        try { await _client.PutAsync($"/api/quotes/{quoteId}/lines/{lineId}/accept", null, ct); }
        catch { /* best effort */ }
    }

    private async Task UpdateQuoteStatusAsync(Guid quoteId, string status, CancellationToken ct)
    {
        try { await _client.PutAsJsonAsync($"/api/quotes/{quoteId}/status", new { status }, ct); }
        catch { /* best effort */ }
    }

    private record IdResponse(Guid Id);
    private record ArticleItem(Guid Id, string Code, string Name, string ArticleType, string? UnitOfMeasure);
    private record OperationTypeItem(Guid Id, string Name, bool IsSubcontracted);
    private record ArticlePagedResult(List<ArticleItem> Items, int TotalCount, int Page, int PageSize, int TotalPages);
}
