namespace Erp.Domain.Articles;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Article> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, bool includeInactive = false, string? search = null, Guid? categoryId = null, string? articleType = null, CancellationToken ct = default);
    Task AddAsync(Article article, CancellationToken ct = default);
    Task UpdateAsync(Article article, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ArticleHistoryEntry>> GetHistoryAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<ArticleCategory>> GetCategoriesAsync(CancellationToken ct = default);
    Task<Guid> AddCategoryAsync(ArticleCategory category, CancellationToken ct = default);

    Task<IReadOnlyList<UnitOfMeasure>> GetUnitsOfMeasureAsync(CancellationToken ct = default);
    Task<Guid> AddUnitOfMeasureAsync(UnitOfMeasure uom, CancellationToken ct = default);

    Task<IReadOnlyList<BomLine>> GetBomAsync(Guid parentArticleId, CancellationToken ct = default);
    Task<BomLine?> GetBomLineAsync(Guid bomLineId, CancellationToken ct = default);
    Task<Guid> AddBomComponentAsync(Guid parentArticleId, Guid childArticleId, decimal quantity, Guid? unitOfMeasureId, int sortOrder, CancellationToken ct = default);
    Task UpdateBomComponentAsync(Guid bomLineId, decimal quantity, Guid? unitOfMeasureId, int sortOrder, CancellationToken ct = default);
    Task RemoveBomComponentAsync(Guid bomLineId, CancellationToken ct = default);

    Task<IReadOnlyList<ArticleOperation>> GetOperationsAsync(Guid articleId, CancellationToken ct = default);
    Task<ArticleOperation?> GetOperationAsync(Guid operationId, CancellationToken ct = default);
    Task AddOperationAsync(ArticleOperation op, CancellationToken ct = default);
    Task UpdateOperationAsync(ArticleOperation op, CancellationToken ct = default);
    Task RemoveOperationAsync(Guid operationId, CancellationToken ct = default);

    Task<IReadOnlyList<OperationType>> GetOperationTypesAsync(CancellationToken ct = default);
    Task<OperationType?> GetOperationTypeAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MachineType>> GetMachineTypesAsync(CancellationToken ct = default);
}

public record ArticleHistoryEntry(
    long Id,
    string EventType,
    string Summary,
    string ChangedBy,
    DateTime ChangedAt
);
