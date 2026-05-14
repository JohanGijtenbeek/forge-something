namespace Erp.Domain.Orders;

public class OrderOperation
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Guid OperationTypeId { get; private set; }
    public string OperationTypeName { get; private set; }
    public bool IsSubcontracted { get; private set; }
    public decimal? EstimatedMinutes { get; private set; }
    public string? Notes { get; private set; }
    public bool IsConditional { get; private set; }

    private OrderOperation() { OperationTypeName = null!; }

    public OrderOperation(Guid orderId, int sequenceNumber, Guid operationTypeId,
        string operationTypeName, bool isSubcontracted,
        decimal? estimatedMinutes, string? notes, bool isConditional)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        SequenceNumber = sequenceNumber;
        OperationTypeId = operationTypeId;
        OperationTypeName = operationTypeName;
        IsSubcontracted = isSubcontracted;
        EstimatedMinutes = estimatedMinutes;
        Notes = notes;
        IsConditional = isConditional;
    }

    public static OrderOperation Reconstitute(
        Guid id, Guid orderId, int sequenceNumber, Guid operationTypeId,
        string operationTypeName, bool isSubcontracted,
        decimal? estimatedMinutes, string? notes, bool isConditional) =>
        new()
        {
            Id = id,
            OrderId = orderId,
            SequenceNumber = sequenceNumber,
            OperationTypeId = operationTypeId,
            OperationTypeName = operationTypeName,
            IsSubcontracted = isSubcontracted,
            EstimatedMinutes = estimatedMinutes,
            Notes = notes,
            IsConditional = isConditional
        };
}
