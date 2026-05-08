using Erp.Domain.Parties;
using Erp.Domain.Search;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Erp.Api.Endpoints;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/search")
            .WithTags("Search")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Results<Ok<IEnumerable<SearchResult>>, BadRequest<string>>> (
            string q,
            ISearchService search,
            int limit = 5,
            CancellationToken ct = default) =>
        {
            if (string.IsNullOrWhiteSpace(q))
                return TypedResults.BadRequest("Query parameter 'q' is verplicht.");

            var results = await search.GlobalSearchAsync(q, limit);
            return TypedResults.Ok(results);
        })
        .WithName("GlobalSearch")
        .WithSummary("Globaal zoeken")
        .WithDescription("Doorzoekt alle geïndexeerde entiteiten. Ondersteunt typo-tolerantie.");

        group.MapPost("/reindex", async (
            IPartyRepository repo,
            ISearchService search,
            CancellationToken ct = default) =>
        {
            var count = await search.ReindexPartiesAsync(repo, ct);
            return Results.Ok($"{count} parties geïndexeerd.");
        })
        .WithRequestTimeout("long")
        .WithName("ReindexParties")
        .WithSummary("Parties herindexeren")
        .WithDescription("Indexeert alle actieve parties opnieuw in Meilisearch. Veilig om meerdere keren aan te roepen.");

        return app;
    }
}
