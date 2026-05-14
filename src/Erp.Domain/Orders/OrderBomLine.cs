namespace Erp.Domain.Orders;

public class OrderBomLine
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ComponentId { get; private set; }
    public string ComponentCode { get; private set; }
    public string ComponentName { get; private set; }
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; }
    public string? Notes { get; private set; }

    private OrderBomLine()
    {
        ComponentCode = null!;
        ComponentName = null!;
        UnitOfMeasure = null!;
    }

    public OrderBomLine(Guid orderId, Guid componentId, string componentCode,
        string componentName, decimal quantity, string unitOfMeasure, string? notes)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ComponentId = componentId;
        ComponentCode = componentCode;
        ComponentName = componentName;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        Notes = notes;
    }

    public static OrderBomLine Reconstitute(
        Guid id, Guid orderId, Guid componentId, string componentCode,
        string componentName, decimal quantity, string unitOfMeasure, string? notes) =>
        new()
        {
            Id = id,
            OrderId = orderId,
            ComponentId = componentId,
            ComponentCode = componentCode,
            ComponentName = componentName,
            Quantity = quantity,
            UnitOfMeasure = unitOfMeasure,
            Notes = notes
        };
}
