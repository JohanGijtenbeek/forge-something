namespace Erp.Domain.Articles;

public class ArticleOperation
{
    public Guid Id { get; private set; }
    public Guid ArticleId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Guid OperationTypeId { get; private set; }
    public string OperationTypeName { get; private set; }
    public bool IsSubcontracted { get; private set; }
    public decimal? EstimatedMinutes { get; private set; }
    public string? Notes { get; private set; }
    public bool IsConditional { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ArticleOperation() { OperationTypeName = null!; }

    public ArticleOperation(
        Guid articleId, int sequenceNumber, Guid operationTypeId,
        string operationTypeName, bool isSubcontracted,
        decimal? estimatedMinutes, string? notes, bool isConditional)
    {
        Id = Guid.NewGuid();
        ArticleId = articleId;
        SequenceNumber = sequenceNumber;
        OperationTypeId = operationTypeId;
        OperationTypeName = operationTypeName;
        IsSubcontracted = isSubcontracted;
        EstimatedMinutes = estimatedMinutes;
        Notes = notes;
        IsConditional = isConditional;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(int sequenceNumber, decimal? estimatedMinutes, string? notes, bool isConditional)
    {
        SequenceNumber = sequenceNumber;
        EstimatedMinutes = estimatedMinutes;
        Notes = notes;
        IsConditional = isConditional;
    }

    public static ArticleOperation Reconstitute(
        Guid id, Guid articleId, int sequenceNumber,
        Guid operationTypeId, string operationTypeName, bool isSubcontracted,
        decimal? estimatedMinutes, string? notes, bool isConditional,
        bool isActive, DateTime createdAt) =>
        new()
        {
            Id = id,
            ArticleId = articleId,
            SequenceNumber = sequenceNumber,
            OperationTypeId = operationTypeId,
            OperationTypeName = operationTypeName,
            IsSubcontracted = isSubcontracted,
            EstimatedMinutes = estimatedMinutes,
            Notes = notes,
            IsConditional = isConditional,
            IsActive = isActive,
            CreatedAt = createdAt
        };
}
