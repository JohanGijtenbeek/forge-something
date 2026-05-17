using MediatR;

namespace Erp.Domain.Orders.Commands;

public record CreateProductionOrderCommand(
    Guid ArticleId,
    Guid? CustomerId,
    decimal Quantity,
    string UnitOfMeasure,
    DateOnly? DueDate,
    string? Notes,
    Guid? QuoteId = null
) : IRequest<Guid>;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    string NewStatus
) : IRequest;

public record CancelOrderCommand(
    Guid OrderId
) : IRequest;
