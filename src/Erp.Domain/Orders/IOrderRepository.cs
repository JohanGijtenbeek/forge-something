namespace Erp.Domain.Orders;

public interface IOrderRepository
{
    Task<ProductionOrder?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<ProductionOrder> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? search, string? status, CancellationToken ct = default);
    Task SaveAsync(ProductionOrder order, IReadOnlyList<OrderBomLine> bom,
        IReadOnlyList<OrderOperation> ops, CancellationToken ct = default);
    Task UpdateStatusAsync(ProductionOrder order, CancellationToken ct = default);
    Task<IReadOnlyList<OrderBomLine>> GetBomLinesAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderOperation>> GetOperationsAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderHistoryEntry>> GetHistoryAsync(Guid orderId, CancellationToken ct = default);
}

public record OrderHistoryEntry(
    long Id,
    string EventType,
    string Summary,
    string ChangedBy,
    DateTime ChangedAt
);
