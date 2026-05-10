using Erp.Domain.Articles;

namespace Erp.Api.Endpoints;

// ============================================================
// RESPONSES
// ============================================================

public record ArticleListResponse(
    Guid Id,
    int ArticleNumber,
    string Code,
    string Name,
    string ArticleType,
    string? Category,
    string? UnitOfMeasure,
    decimal? PurchasePrice,
    bool IsActive
);

public record ArticleDetailResponse(
    Guid Id,
    int ArticleNumber,
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    string? Category,
    Guid? UnitOfMeasureId,
    string? UnitOfMeasure,
    decimal? PurchasePrice,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ArticleHistoryEntryResponse(
    long Id,
    string EventType,
    string Summary,
    string ChangedBy,
    DateTime ChangedAt
);

public record ArticleCategoryResponse(
    Guid Id,
    string Name,
    int SortOrder,
    bool IsActive
);

public record UnitOfMeasureResponse(
    Guid Id,
    string Name,
    string Abbreviation,
    bool IsActive
);

public record BomLineResponse(
    Guid Id,
    Guid ChildArticleId,
    string ChildCode,
    string ChildName,
    string ChildArticleType,
    decimal Quantity,
    Guid? UnitOfMeasureId,
    string? UnitOfMeasure,
    int SortOrder
);

// ============================================================
// REQUESTS
// ============================================================

public record CreateArticleRequest(
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    Guid? UnitOfMeasureId,
    decimal? PurchasePrice
);

public record UpdateArticleRequest(
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    Guid? UnitOfMeasureId,
    decimal? PurchasePrice
);

public record CreateArticleCategoryRequest(
    string Name,
    int SortOrder = 0
);

public record CreateUnitOfMeasureRequest(
    string Name,
    string Abbreviation
);

public record AddBomComponentRequest(
    Guid ChildArticleId,
    decimal Quantity,
    Guid? UnitOfMeasureId,
    int SortOrder = 0
);

public record UpdateBomComponentRequest(
    decimal Quantity,
    Guid? UnitOfMeasureId,
    int SortOrder
);

// ============================================================
// MAPPER
// ============================================================

public static class ArticleMapper
{
    public static ArticleListResponse ToListResponse(Article a) =>
        new(a.Id, a.ArticleNumber, a.Code, a.Name, a.ArticleType, a.CategoryName, a.UomAbbreviation, a.PurchasePrice, a.IsActive);

    public static ArticleDetailResponse ToDetailResponse(Article a) =>
        new(a.Id, a.ArticleNumber, a.Code, a.Name, a.ArticleType, a.Description,
            a.CategoryId, a.CategoryName, a.UnitOfMeasureId, a.UomAbbreviation, a.PurchasePrice, a.IsActive, a.CreatedAt, a.UpdatedAt);

    public static BomLineResponse ToBomLineResponse(BomLine b) =>
        new(b.Id, b.ChildArticleId, b.ChildCode, b.ChildName, b.ChildArticleType,
            b.Quantity, b.UnitOfMeasureId, b.UnitOfMeasureAbbreviation, b.SortOrder);
}
