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
public static class SearchEntityTypes
{
    public const string Party = "party";
    public const string Order = "order";
    public const string Article = "article";
}
