namespace Erp.Domain.Search;

// Uniform resultaatmodel voor global search
// EntityType bepaalt waar de frontend naartoe navigeert
public record SearchResult(
    string Id,
    string EntityType,
    string DisplayLabel,
    string? Subtitle = null
);

// Ondersteunde entiteitstypen voor global search
public record QuoteSearchDocument(
    string Id,
    int QuoteNumber,
    string? CustomerName,
    string Status
)
{
    public string EntityType => SearchEntityTypes.Quote;
    public string DisplayLabel => $"#{QuoteNumber} — {CustomerName ?? "No customer"}";
}

public static class SearchEntityTypes
{
    public const string Party   = "party";
    public const string Order   = "order";
    public const string Article = "article";
    public const string Quote   = "quote";
}
