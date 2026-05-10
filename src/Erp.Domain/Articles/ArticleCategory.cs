namespace Erp.Domain.Articles;

public class ArticleCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ArticleCategory() { Name = null!; }

    public ArticleCategory(string name, int sortOrder)
    {
        Id = Guid.NewGuid();
        Name = name;
        SortOrder = sortOrder;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ArticleCategory Reconstitute(
        Guid id, string name, int sortOrder, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        return new ArticleCategory
        {
            Id = id,
            Name = name,
            SortOrder = sortOrder,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
