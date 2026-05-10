namespace Erp.Domain.Articles;

public class BomLine
{
    public Guid Id { get; private set; }
    public Guid ParentArticleId { get; private set; }
    public Guid ChildArticleId { get; private set; }
    public string ChildCode { get; private set; }
    public string ChildName { get; private set; }
    public string ChildArticleType { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid? UnitOfMeasureId { get; private set; }
    public string? UnitOfMeasureAbbreviation { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    private BomLine()
    {
        ChildCode = null!;
        ChildName = null!;
        ChildArticleType = null!;
    }

    public static BomLine Reconstitute(
        Guid id, Guid parentArticleId, Guid childArticleId,
        string childCode, string childName, string childArticleType,
        decimal quantity, Guid? unitOfMeasureId, string? unitOfMeasureAbbreviation,
        int sortOrder, bool isActive)
    {
        return new BomLine
        {
            Id = id,
            ParentArticleId = parentArticleId,
            ChildArticleId = childArticleId,
            ChildCode = childCode,
            ChildName = childName,
            ChildArticleType = childArticleType,
            Quantity = quantity,
            UnitOfMeasureId = unitOfMeasureId,
            UnitOfMeasureAbbreviation = unitOfMeasureAbbreviation,
            SortOrder = sortOrder,
            IsActive = isActive
        };
    }
}
