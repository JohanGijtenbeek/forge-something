using Erp.Domain.Parties;

namespace Erp.Domain.Search;

// Interface in Domain zodat Application/Domain nooit afhankelijk is van Meilisearch
public interface ISearchService
{
    Task<IEnumerable<SearchResult>> GlobalSearchAsync(string query, int limit = 5);
    Task IndexPartyAsync(PartySearchDocument document);
    Task DeletePartyAsync(string id);
    Task IndexArticleAsync(ArticleSearchDocument document);
    Task DeleteArticleAsync(string id);
    Task InitializeAsync();
    Task<int> ReindexPartiesAsync(IPartyRepository repository, CancellationToken ct = default);
}

// Zoekdocument voor parties - alleen de velden die Meilisearch doorzoekt
// Bewust plat gehouden, niet het volledige domeinmodel
public record PartySearchDocument(
    string Id,
    string Name,
    string? City,
    string? Email,
    string? Phone,
    string[] Roles, // ["customer", "supplier"]
    bool IsActive
)
{
    public string EntityType => SearchEntityTypes.Party;
    public string DisplayLabel => Name;
}

public record ArticleSearchDocument(
    string Id,
    string Code,
    string Name,
    string? Category,
    bool IsActive
)
{
    public string EntityType => SearchEntityTypes.Article;
    public string DisplayLabel => $"{Code} - {Name}";
}
