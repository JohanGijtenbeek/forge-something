namespace Erp.Domain.Orders;

public class ProductionOrder
{
    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public Guid ArticleId { get; private set; }
    public string ArticleCode { get; private set; }
    public string ArticleName { get; private set; }
    public string? ArticleRevision { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; }
    public string Status { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProductionOrder()
    {
        ArticleCode = null!;
        ArticleName = null!;
        UnitOfMeasure = null!;
        Status = null!;
    }

    public ProductionOrder(int orderNumber, Guid articleId, string articleCode,
        string articleName, string? articleRevision, Guid? customerId, string? customerName,
        decimal quantity, string unitOfMeasure, DateOnly? dueDate, string? notes)
    {
        Id = Guid.NewGuid();
        OrderNumber = orderNumber;
        ArticleId = articleId;
        ArticleCode = articleCode;
        ArticleName = articleName;
        ArticleRevision = articleRevision;
        CustomerId = customerId;
        CustomerName = customerName;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        Status = OrderStatus.Draft;
        DueDate = dueDate;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void TransitionTo(string newStatus)
    {
        if (!OrderStatus.CanTransitionTo(Status, newStatus))
            throw new InvalidOperationException(
                $"Cannot transition from '{Status}' to '{newStatus}'.");
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ProductionOrder Reconstitute(
        Guid id, int orderNumber, Guid articleId, string articleCode,
        string articleName, string? articleRevision, Guid? customerId, string? customerName,
        decimal quantity, string unitOfMeasure, string status, DateOnly? dueDate,
        string? notes, DateTime createdAt, DateTime updatedAt) =>
        new()
        {
            Id = id,
            OrderNumber = orderNumber,
            ArticleId = articleId,
            ArticleCode = articleCode,
            ArticleName = articleName,
            ArticleRevision = articleRevision,
            CustomerId = customerId,
            CustomerName = customerName,
            Quantity = quantity,
            UnitOfMeasure = unitOfMeasure,
            Status = status,
            DueDate = dueDate,
            Notes = notes,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
}
