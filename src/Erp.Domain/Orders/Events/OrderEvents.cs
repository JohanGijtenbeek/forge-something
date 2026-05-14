namespace Erp.Domain.Orders.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    int OrderNumber,
    Guid ArticleId,
    string ArticleCode,
    string ArticleName,
    Guid? CustomerId,
    string? CustomerName,
    decimal Quantity,
    string UnitOfMeasure,
    DateOnly? DueDate,
    DateTime OccurredAt
);

public record OrderStatusChangedEvent(
    Guid OrderId,
    int OrderNumber,
    string OldStatus,
    string NewStatus,
    DateTime OccurredAt
);
