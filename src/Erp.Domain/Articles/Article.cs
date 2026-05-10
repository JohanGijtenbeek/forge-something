namespace Erp.Domain.Articles;

public class Article
{
    public Guid Id { get; private set; }
    public int ArticleNumber { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string ArticleType { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? UnitOfMeasureId { get; private set; }
    public decimal? PurchasePrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Resolved names — only populated when fetched with JOIN (nullable when fetched without)
    public string? CategoryName { get; private set; }
    public string? UomAbbreviation { get; private set; }

    private Article()
    {
        Code = null!;
        Name = null!;
        ArticleType = null!;
    }

    public Article(string code, string name, string articleType, string? description,
        Guid? categoryId, Guid? unitOfMeasureId, decimal? purchasePrice)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        ArticleType = articleType;
        Description = description;
        CategoryId = categoryId;
        UnitOfMeasureId = unitOfMeasureId;
        PurchasePrice = purchasePrice;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string code, string name, string articleType, string? description,
        Guid? categoryId, Guid? unitOfMeasureId, decimal? purchasePrice)
    {
        Code = code;
        Name = name;
        ArticleType = articleType;
        Description = description;
        CategoryId = categoryId;
        UnitOfMeasureId = unitOfMeasureId;
        PurchasePrice = purchasePrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Article Reconstitute(
        Guid id, int articleNumber, string code, string name, string articleType,
        string? description, Guid? categoryId, Guid? unitOfMeasureId,
        decimal? purchasePrice, bool isActive, DateTime createdAt, DateTime updatedAt,
        string? categoryName = null, string? uomAbbreviation = null)
    {
        return new Article
        {
            Id = id,
            ArticleNumber = articleNumber,
            Code = code,
            Name = name,
            ArticleType = articleType,
            Description = description,
            CategoryId = categoryId,
            UnitOfMeasureId = unitOfMeasureId,
            PurchasePrice = purchasePrice,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            CategoryName = categoryName,
            UomAbbreviation = uomAbbreviation
        };
    }
}
