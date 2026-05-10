using Dapper;
using Erp.Domain.Articles;
using Erp.Infrastructure.Persistence;

namespace Erp.Infrastructure.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly DbConnectionFactory _factory;

    public ArticleRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    private const string ArticleSelect = @"
        SELECT a.id, a.article_number, a.code, a.name, a.article_type,
               a.description, a.category_id, a.unit_of_measure_id,
               a.purchase_price, a.is_active, a.created_at, a.updated_at,
               c.name AS category_name,
               u.abbreviation AS uom_abbreviation
        FROM mdata.articles a
        LEFT JOIN mdata.article_categories c ON c.id = a.category_id
        LEFT JOIN mdata.units_of_measure u ON u.id = a.unit_of_measure_id";

    public async Task<Article?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<ArticleRow>(
            $"{ArticleSelect} WHERE a.id = @Id",
            new { Id = id });
        return row?.ToDomain();
    }

    public async Task<(IReadOnlyList<Article> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, bool includeInactive = false,
        string? search = null, Guid? categoryId = null, string? articleType = null,
        CancellationToken ct = default)
    {
        using var conn = _factory.Create();

        var where = new System.Text.StringBuilder("WHERE (@IncludeInactive = 1 OR a.is_active = 1)");
        if (!string.IsNullOrWhiteSpace(search))
            where.Append(" AND (a.code LIKE @Search OR a.name LIKE @Search)");
        if (categoryId.HasValue)
            where.Append(" AND a.category_id = @CategoryId");
        if (!string.IsNullOrWhiteSpace(articleType))
            where.Append(" AND a.article_type = @ArticleType");

        var sql = $@"
            SELECT COUNT(*) FROM mdata.articles a {where};

            SELECT a.id, a.article_number, a.code, a.name, a.article_type,
                   a.description, a.category_id, a.unit_of_measure_id,
                   a.purchase_price, a.is_active, a.created_at, a.updated_at,
                   c.name AS category_name,
                   u.abbreviation AS uom_abbreviation
            FROM mdata.articles a
            LEFT JOIN mdata.article_categories c ON c.id = a.category_id
            LEFT JOIN mdata.units_of_measure u ON u.id = a.unit_of_measure_id
            {where}
            ORDER BY a.name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        using var multi = await conn.QueryMultipleAsync(sql, new
        {
            IncludeInactive = includeInactive ? 1 : 0,
            Search = $"%{search}%",
            CategoryId = categoryId,
            ArticleType = articleType,
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        });

        var totalCount = await multi.ReadSingleAsync<int>();
        var rows = await multi.ReadAsync<ArticleRow>();
        return (rows.Select(r => r.ToDomain()).ToList(), totalCount);
    }

    public async Task AddAsync(Article article, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO mdata.articles
                (id, code, name, article_type, description, category_id, unit_of_measure_id, purchase_price, is_active, created_at, updated_at)
            VALUES
                (@Id, @Code, @Name, @ArticleType, @Description, @CategoryId, @UnitOfMeasureId, @PurchasePrice, @IsActive, @CreatedAt, @UpdatedAt)",
            new
            {
                article.Id, article.Code, article.Name, article.ArticleType,
                article.Description, article.CategoryId, article.UnitOfMeasureId,
                article.PurchasePrice, article.IsActive, article.CreatedAt, article.UpdatedAt
            });
    }

    public async Task UpdateAsync(Article article, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            UPDATE mdata.articles
            SET code               = @Code,
                name               = @Name,
                article_type       = @ArticleType,
                description        = @Description,
                category_id        = @CategoryId,
                unit_of_measure_id = @UnitOfMeasureId,
                purchase_price     = @PurchasePrice,
                updated_at         = @UpdatedAt
            WHERE id = @Id",
            new
            {
                article.Id, article.Code, article.Name, article.ArticleType,
                article.Description, article.CategoryId, article.UnitOfMeasureId,
                article.PurchasePrice, article.UpdatedAt
            });
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE mdata.articles SET is_active = 0, updated_at = SYSUTCDATETIME() WHERE id = @Id",
            new { Id = id });
    }

    public async Task<IReadOnlyList<ArticleHistoryEntry>> GetHistoryAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<ArticleHistoryEntry>(@"
            SELECT id, event_type AS EventType, summary AS Summary,
                   changed_by AS ChangedBy, changed_at AS ChangedAt
            FROM audit.article_history
            WHERE article_id = @Id
            ORDER BY changed_at DESC",
            new { Id = id });
        return rows.ToList();
    }

    public async Task<IReadOnlyList<ArticleCategory>> GetCategoriesAsync(CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<ArticleCategoryRow>(
            "SELECT * FROM mdata.article_categories WHERE is_active = 1 ORDER BY sort_order, name");
        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Guid> AddCategoryAsync(ArticleCategory category, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO mdata.article_categories (id, name, sort_order, is_active, created_at, updated_at)
            VALUES (@Id, @Name, @SortOrder, @IsActive, @CreatedAt, @UpdatedAt)",
            new { category.Id, category.Name, category.SortOrder, category.IsActive, category.CreatedAt, category.UpdatedAt });
        return category.Id;
    }

    public async Task<IReadOnlyList<UnitOfMeasure>> GetUnitsOfMeasureAsync(CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<UnitOfMeasureRow>(
            "SELECT * FROM mdata.units_of_measure WHERE is_active = 1 ORDER BY name");
        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Guid> AddUnitOfMeasureAsync(UnitOfMeasure uom, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO mdata.units_of_measure (id, name, abbreviation, is_active, created_at, updated_at)
            VALUES (@Id, @Name, @Abbreviation, @IsActive, @CreatedAt, @UpdatedAt)",
            new { uom.Id, uom.Name, uom.Abbreviation, uom.IsActive, uom.CreatedAt, uom.UpdatedAt });
        return uom.Id;
    }

    public async Task<IReadOnlyList<BomLine>> GetBomAsync(Guid parentArticleId, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<BomLineRow>(@"
            SELECT b.id, b.parent_article_id, b.child_article_id,
                   a.code  AS child_code,
                   a.name  AS child_name,
                   a.article_type AS child_article_type,
                   b.quantity,
                   b.unit_of_measure_id,
                   u.abbreviation AS unit_of_measure_abbreviation,
                   b.sort_order,
                   b.is_active
            FROM mdata.bill_of_materials b
            JOIN mdata.articles a ON a.id = b.child_article_id
            LEFT JOIN mdata.units_of_measure u ON u.id = b.unit_of_measure_id
            WHERE b.parent_article_id = @ParentArticleId AND b.is_active = 1
            ORDER BY b.sort_order, a.name",
            new { ParentArticleId = parentArticleId });
        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task<BomLine?> GetBomLineAsync(Guid bomLineId, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<BomLineRow>(@"
            SELECT b.id, b.parent_article_id, b.child_article_id,
                   a.code  AS child_code,
                   a.name  AS child_name,
                   a.article_type AS child_article_type,
                   b.quantity,
                   b.unit_of_measure_id,
                   u.abbreviation AS unit_of_measure_abbreviation,
                   b.sort_order,
                   b.is_active
            FROM mdata.bill_of_materials b
            JOIN mdata.articles a ON a.id = b.child_article_id
            LEFT JOIN mdata.units_of_measure u ON u.id = b.unit_of_measure_id
            WHERE b.id = @Id",
            new { Id = bomLineId });
        return row?.ToDomain();
    }

    public async Task<Guid> AddBomComponentAsync(Guid parentArticleId, Guid childArticleId,
        decimal quantity, Guid? unitOfMeasureId, int sortOrder, CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO mdata.bill_of_materials
                (id, parent_article_id, child_article_id, quantity, unit_of_measure_id, sort_order)
            VALUES
                (@Id, @ParentArticleId, @ChildArticleId, @Quantity, @UnitOfMeasureId, @SortOrder)",
            new { Id = id, ParentArticleId = parentArticleId, ChildArticleId = childArticleId,
                  Quantity = quantity, UnitOfMeasureId = unitOfMeasureId, SortOrder = sortOrder });
        return id;
    }

    public async Task UpdateBomComponentAsync(Guid bomLineId, decimal quantity,
        Guid? unitOfMeasureId, int sortOrder, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            UPDATE mdata.bill_of_materials
            SET quantity = @Quantity, unit_of_measure_id = @UnitOfMeasureId, sort_order = @SortOrder
            WHERE id = @Id",
            new { Id = bomLineId, Quantity = quantity, UnitOfMeasureId = unitOfMeasureId, SortOrder = sortOrder });
    }

    public async Task RemoveBomComponentAsync(Guid bomLineId, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE mdata.bill_of_materials SET is_active = 0 WHERE id = @Id",
            new { Id = bomLineId });
    }
}

// ============================================================
// Dapper row models
// ============================================================

file record ArticleRow
{
    public Guid Id { get; init; }
    public int ArticleNumber { get; init; }
    public string Code { get; init; } = "";
    public string Name { get; init; } = "";
    public string ArticleType { get; init; } = "";
    public string? Description { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? UnitOfMeasureId { get; init; }
    public decimal? PurchasePrice { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string? CategoryName { get; init; }
    public string? UomAbbreviation { get; init; }

    public Article ToDomain() => Article.Reconstitute(
        Id, ArticleNumber, Code, Name, ArticleType, Description,
        CategoryId, UnitOfMeasureId, PurchasePrice, IsActive, CreatedAt, UpdatedAt,
        CategoryName, UomAbbreviation);
}

file record ArticleCategoryRow
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public ArticleCategory ToDomain() =>
        ArticleCategory.Reconstitute(Id, Name, SortOrder, IsActive, CreatedAt, UpdatedAt);
}

file record UnitOfMeasureRow
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Abbreviation { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public UnitOfMeasure ToDomain() =>
        UnitOfMeasure.Reconstitute(Id, Name, Abbreviation, IsActive, CreatedAt, UpdatedAt);
}

file record BomLineRow
{
    public Guid Id { get; init; }
    public Guid ParentArticleId { get; init; }
    public Guid ChildArticleId { get; init; }
    public string ChildCode { get; init; } = "";
    public string ChildName { get; init; } = "";
    public string ChildArticleType { get; init; } = "";
    public decimal Quantity { get; init; }
    public Guid? UnitOfMeasureId { get; init; }
    public string? UnitOfMeasureAbbreviation { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }

    public BomLine ToDomain() => BomLine.Reconstitute(
        Id, ParentArticleId, ChildArticleId, ChildCode, ChildName, ChildArticleType,
        Quantity, UnitOfMeasureId, UnitOfMeasureAbbreviation, SortOrder, IsActive);
}
