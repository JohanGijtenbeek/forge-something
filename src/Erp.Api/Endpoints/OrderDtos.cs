using Erp.Domain.Orders;

namespace Erp.Api.Endpoints;

// ============================================================
// RESPONSES
// ============================================================

public record OrderSummaryResponse(
    Guid Id,
    int OrderNumber,
    string ArticleCode,
    string ArticleName,
    string? CustomerName,
    decimal Quantity,
    string UnitOfMeasure,
    string Status,
    DateOnly? DueDate,
    DateTime CreatedAt
);

public record OrderDetailResponse(
    Guid Id,
    int OrderNumber,
    Guid ArticleId,
    string ArticleCode,
    string ArticleName,
    string? ArticleRevision,
    Guid? CustomerId,
    string? CustomerName,
    decimal Quantity,
    string UnitOfMeasure,
    string Status,
    DateOnly? DueDate,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<OrderBomLineResponse> BomLines,
    IEnumerable<OrderOperationResponse> Operations
);

public record OrderBomLineResponse(
    Guid Id,
    Guid ComponentId,
    string ComponentCode,
    string ComponentName,
    decimal Quantity,
    string UnitOfMeasure,
    string? Notes
);

public record OrderOperationResponse(
    Guid Id,
    int SequenceNumber,
    Guid OperationTypeId,
    string OperationTypeName,
    bool IsSubcontracted,
    decimal? EstimatedMinutes,
    string? Notes,
    bool IsConditional
);

public record OrderHistoryEntryResponse(
    long Id,
    string EventType,
    string Summary,
    string ChangedBy,
    DateTime ChangedAt
);

// ============================================================
// REQUESTS
// ============================================================

public record CreateOrderRequest(
    Guid ArticleId,
    Guid? CustomerId,
    decimal Quantity,
    string UnitOfMeasure,
    DateOnly? DueDate,
    string? Notes
);

public record UpdateOrderStatusRequest(
    string Status
);

// ============================================================
// MAPPER
// ============================================================

public static class OrderMapper
{
    public static OrderSummaryResponse ToSummaryResponse(ProductionOrder o) =>
        new(o.Id, o.OrderNumber, o.ArticleCode, o.ArticleName, o.CustomerName,
            o.Quantity, o.UnitOfMeasure, o.Status, o.DueDate, o.CreatedAt);

    public static OrderBomLineResponse ToBomLineResponse(OrderBomLine b) =>
        new(b.Id, b.ComponentId, b.ComponentCode, b.ComponentName,
            b.Quantity, b.UnitOfMeasure, b.Notes);

    public static OrderOperationResponse ToOperationResponse(OrderOperation op) =>
        new(op.Id, op.SequenceNumber, op.OperationTypeId, op.OperationTypeName,
            op.IsSubcontracted, op.EstimatedMinutes, op.Notes, op.IsConditional);

    public static OrderDetailResponse ToDetailResponse(
        ProductionOrder o,
        IEnumerable<OrderBomLine> bom,
        IEnumerable<OrderOperation> ops) =>
        new(o.Id, o.OrderNumber, o.ArticleId, o.ArticleCode, o.ArticleName, o.ArticleRevision,
            o.CustomerId, o.CustomerName, o.Quantity, o.UnitOfMeasure, o.Status,
            o.DueDate, o.Notes, o.CreatedAt, o.UpdatedAt,
            bom.Select(ToBomLineResponse),
            ops.Select(ToOperationResponse));
}
