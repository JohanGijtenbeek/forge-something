using Bogus;
using Erp.Seeder.Models;
using System.Net.Http.Json;

namespace Erp.Seeder.Generators;

public class OrderGenerator
{
    private readonly Faker _faker;
    private readonly HttpClient _client;

    public OrderGenerator(int seed, string apiUrl)
    {
        _faker = new Faker { Random = new Randomizer(seed + 300) };
        _client = new HttpClient { BaseAddress = new Uri(apiUrl) };
    }

    public async Task<List<OrderSeedRow>> GenerateAsync(int count, CancellationToken ct = default)
    {
        // Fetch manufactured articles
        var articlesResponse = await _client.GetFromJsonAsync<PagedResult<ArticleListItem>>(
            "/api/articles?articleType=manufactured&pageSize=500&page=1", ct);
        var articles = articlesResponse?.Items ?? [];

        if (articles.Count == 0)
        {
            Console.WriteLine("  ⚠ No manufactured articles found — skipping order generation.");
            return [];
        }

        // Fetch customers (returns plain list, not paged)
        var customers = await _client.GetFromJsonAsync<List<PartyListItem>>(
            "/api/parties/customers", ct) ?? [];

        var orders = new List<OrderSeedRow>();

        for (var i = 0; i < count; i++)
        {
            var article = _faker.PickRandom(articles);
            Guid? customerId = customers.Count > 0 && _faker.Random.Bool(0.8f)
                ? _faker.PickRandom(customers).Id
                : null;

            var daysFromNow = _faker.Random.Int(30, 365);
            var dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow));

            orders.Add(new OrderSeedRow(
                ArticleId: article.Id,
                CustomerId: customerId,
                Quantity: Math.Round((decimal)_faker.Random.Double(1, 500), 4),
                UnitOfMeasure: article.UnitOfMeasure ?? "st",
                DueDate: dueDate,
                Notes: _faker.Random.Bool(0.3f) ? _faker.Lorem.Sentence() : null
            ));
        }

        return orders;
    }

    private record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize, int TotalPages);
    private record ArticleListItem(Guid Id, string Code, string Name, string? UnitOfMeasure);
    private record PartyListItem(Guid Id, string Name);
}
