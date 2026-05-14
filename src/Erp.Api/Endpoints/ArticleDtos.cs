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
    string? Revision,
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

public record ArticleOperationResponse(
    Guid Id,
    int SequenceNumber,
    Guid OperationTypeId,
    string OperationTypeName,
    bool IsSubcontracted,
    decimal? EstimatedMinutes,
    string? Notes,
    bool IsConditional
);

public record OperationTypeResponse(
    Guid Id,
    string Name,
    bool IsSubcontracted,
    Guid? MachineTypeId,
    string? MachineTypeName,
    bool IsActive
);

public record MachineTypeResponse(
    Guid Id,
    string Name,
    bool IsActive
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
    decimal? PurchasePrice,
    string? Revision = null
);

public record UpdateArticleRequest(
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    Guid? UnitOfMeasureId,
    decimal? PurchasePrice,
    string? Revision = null
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

public record AddArticleOperationRequest(
    int SequenceNumber,
    Guid OperationTypeId,
    decimal? EstimatedMinutes,
    string? Notes,
    bool IsConditional = false
);

public record UpdateArticleOperationRequest(
    int SequenceNumber,
    decimal? EstimatedMinutes,
    string? Notes,
    bool IsConditional
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
            a.CategoryId, a.CategoryName, a.UnitOfMeasureId, a.UomAbbreviation, a.PurchasePrice,
            a.Revision, a.IsActive, a.CreatedAt, a.UpdatedAt);

    public static BomLineResponse ToBomLineResponse(BomLine b) =>
        new(b.Id, b.ChildArticleId, b.ChildCode, b.ChildName, b.ChildArticleType,
            b.Quantity, b.UnitOfMeasureId, b.UnitOfMeasureAbbreviation, b.SortOrder);

    public static ArticleOperationResponse ToOperationResponse(ArticleOperation op) =>
        new(op.Id, op.SequenceNumber, op.OperationTypeId, op.OperationTypeName,
            op.IsSubcontracted, op.EstimatedMinutes, op.Notes, op.IsConditional);

    public static OperationTypeResponse ToOperationTypeResponse(OperationType ot) =>
        new(ot.Id, ot.Name, ot.IsSubcontracted, ot.MachineTypeId, ot.MachineTypeName, ot.IsActive);

    public static MachineTypeResponse ToMachineTypeResponse(MachineType mt) =>
        new(mt.Id, mt.Name, mt.IsActive);
}
