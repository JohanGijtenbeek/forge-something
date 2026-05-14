using Dapper;
using Erp.Domain.Orders;
using Erp.Infrastructure.Persistence;

namespace Erp.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DbConnectionFactory _factory;

    public OrderRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    private const string OrderSelect = @"
        SELECT id, order_number, article_id, article_code, article_name, article_revision,
               customer_id, customer_name, quantity, unit_of_measure, status,
               due_date, notes, created_at, updated_at
        FROM mdata.production_orders";

    public async Task<ProductionOrder?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<OrderRow>(
            $"{OrderSelect} WHERE id = @Id",
            new { Id = id });
        return row?.ToDomain();
    }

    public async Task<(IReadOnlyList<ProductionOrder> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? search, string? status, CancellationToken ct = default)
    {
        using var conn = _factory.Create();

        var where = new System.Text.StringBuilder("WHERE 1=1");
        if (!string.IsNullOrWhiteSpace(status))
            where.Append(" AND status = @Status");
        if (!string.IsNullOrWhiteSpace(search))
            where.Append(" AND (CAST(order_number AS NVARCHAR) LIKE @Search OR article_name LIKE @Search OR customer_name LIKE @Search OR article_code LIKE @Search)");

        var sql = $@"
            SELECT COUNT(*) FROM mdata.production_orders {where};

            SELECT id, order_number, article_id, article_code, article_name, article_revision,
                   customer_id, customer_name, quantity, unit_of_measure, status,
                   due_date, notes, created_at, updated_at
            FROM mdata.production_orders
            {where}
            ORDER BY created_at DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        using var multi = await conn.QueryMultipleAsync(sql, new
        {
            Status = status,
            Search = $"%{search}%",
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        });

        var total = await multi.ReadSingleAsync<int>();
        var rows = await multi.ReadAsync<OrderRow>();
        return (rows.Select(r => r.ToDomain()).ToList(), total);
    }

    public async Task SaveAsync(ProductionOrder order, IReadOnlyList<OrderBomLine> bom,
        IReadOnlyList<OrderOperation> ops, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            await conn.ExecuteAsync(@"
                INSERT INTO mdata.production_orders
                    (id, order_number, article_id, article_code, article_name, article_revision,
                     customer_id, customer_name, quantity, unit_of_measure, status,
                     due_date, notes, created_at, updated_at)
                VALUES
                    (@Id, @OrderNumber, @ArticleId, @ArticleCode, @ArticleName, @ArticleRevision,
                     @CustomerId, @CustomerName, @Quantity, @UnitOfMeasure, @Status,
                     @DueDate, @Notes, @CreatedAt, @UpdatedAt)",
                new
                {
                    order.Id, order.OrderNumber, order.ArticleId, order.ArticleCode,
                    order.ArticleName, order.ArticleRevision, order.CustomerId, order.CustomerName,
                    order.Quantity, order.UnitOfMeasure, order.Status,
                    order.DueDate, order.Notes, order.CreatedAt, order.UpdatedAt
                }, tx);

            if (bom.Count > 0)
            {
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.order_bom_lines
                        (id, order_id, component_id, component_code, component_name,
                         quantity, unit_of_measure, notes)
                    VALUES
                        (@Id, @OrderId, @ComponentId, @ComponentCode, @ComponentName,
                         @Quantity, @UnitOfMeasure, @Notes)",
                    bom.Select(b => new
                    {
                        b.Id, b.OrderId, b.ComponentId, b.ComponentCode, b.ComponentName,
                        b.Quantity, b.UnitOfMeasure, b.Notes
                    }), tx);
            }

            if (ops.Count > 0)
            {
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.order_operations
                        (id, order_id, sequence_number, operation_type_id, operation_type_name,
                         is_subcontracted, estimated_minutes, notes, is_conditional)
                    VALUES
                        (@Id, @OrderId, @SequenceNumber, @OperationTypeId, @OperationTypeName,
                         @IsSubcontracted, @EstimatedMinutes, @Notes, @IsConditional)",
                    ops.Select(o => new
                    {
                        o.Id, o.OrderId, o.SequenceNumber, o.OperationTypeId, o.OperationTypeName,
                        o.IsSubcontracted, o.EstimatedMinutes, o.Notes, o.IsConditional
                    }), tx);
            }

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateStatusAsync(ProductionOrder order, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE mdata.production_orders SET status = @Status, updated_at = @UpdatedAt WHERE id = @Id",
            new { order.Status, order.UpdatedAt, order.Id });
    }

    public async Task<IReadOnlyList<OrderBomLine>> GetBomLinesAsync(Guid orderId, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<OrderBomLineRow>(
            "SELECT id, order_id, component_id, component_code, component_name, quantity, unit_of_measure, notes FROM mdata.order_bom_lines WHERE order_id = @Id",
            new { Id = orderId });
        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<OrderOperation>> GetOperationsAsync(Guid orderId, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<OrderOperationRow>(@"
            SELECT id, order_id, sequence_number, operation_type_id, operation_type_name,
                   is_subcontracted, estimated_minutes, notes, is_conditional
            FROM mdata.order_operations
            WHERE order_id = @Id
            ORDER BY sequence_number",
            new { Id = orderId });
        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<OrderHistoryEntry>> GetHistoryAsync(Guid orderId, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<OrderHistoryEntry>(@"
            SELECT id, event_type AS EventType, summary AS Summary,
                   changed_by AS ChangedBy, changed_at AS ChangedAt
            FROM audit.order_history
            WHERE order_id = @Id
            ORDER BY changed_at DESC",
            new { Id = orderId });
        return rows.ToList();
    }
}

// ============================================================
// Dapper row models
// ============================================================

file record OrderRow
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public Guid ArticleId { get; init; }
    public string ArticleCode { get; init; } = "";
    public string ArticleName { get; init; } = "";
    public string? ArticleRevision { get; init; }
    public Guid? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = "";
    public string Status { get; init; } = "";
    public DateOnly? DueDate { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public ProductionOrder ToDomain() => ProductionOrder.Reconstitute(
        Id, OrderNumber, ArticleId, ArticleCode, ArticleName, ArticleRevision,
        CustomerId, CustomerName, Quantity, UnitOfMeasure, Status,
        DueDate, Notes, CreatedAt, UpdatedAt);
}

file record OrderBomLineRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid ComponentId { get; init; }
    public string ComponentCode { get; init; } = "";
    public string ComponentName { get; init; } = "";
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = "";
    public string? Notes { get; init; }

    public OrderBomLine ToDomain() => OrderBomLine.Reconstitute(
        Id, OrderId, ComponentId, ComponentCode, ComponentName, Quantity, UnitOfMeasure, Notes);
}

file record OrderOperationRow
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public int SequenceNumber { get; init; }
    public Guid OperationTypeId { get; init; }
    public string OperationTypeName { get; init; } = "";
    public bool IsSubcontracted { get; init; }
    public decimal? EstimatedMinutes { get; init; }
    public string? Notes { get; init; }
    public bool IsConditional { get; init; }

    public OrderOperation ToDomain() => OrderOperation.Reconstitute(
        Id, OrderId, SequenceNumber, OperationTypeId, OperationTypeName,
        IsSubcontracted, EstimatedMinutes, Notes, IsConditional);
}
