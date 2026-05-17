using Dapper;
using Erp.Domain.Articles;
using Erp.Domain.Orders;
using Erp.Domain.Orders.Commands;
using Erp.Domain.Orders.Events;
using Erp.Domain.Parties;
using Erp.Infrastructure.Persistence;
using MassTransit;
using MediatR;

namespace Erp.Infrastructure.Handlers;

public class CreateProductionOrderHandler : IRequestHandler<CreateProductionOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IArticleRepository _articleRepo;
    private readonly IPartyRepository _partyRepo;
    private readonly IBus _bus;
    private readonly DbConnectionFactory _factory;

    public CreateProductionOrderHandler(
        IOrderRepository orderRepo, IArticleRepository articleRepo,
        IPartyRepository partyRepo, IBus bus, DbConnectionFactory factory)
    {
        _orderRepo = orderRepo;
        _articleRepo = articleRepo;
        _partyRepo = partyRepo;
        _bus = bus;
        _factory = factory;
    }

    public async Task<Guid> Handle(CreateProductionOrderCommand command, CancellationToken ct)
    {
        var article = await _articleRepo.GetByIdAsync(command.ArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ArticleId} not found.");

        if (article.ArticleType != ArticleType.Manufactured)
            throw new InvalidOperationException("Production orders can only be created for articles of type 'manufactured'.");

        var bom = await _articleRepo.GetBomAsync(command.ArticleId, ct);
        var ops = await _articleRepo.GetOperationsAsync(command.ArticleId, ct);

        string? customerName = null;
        if (command.CustomerId.HasValue)
        {
            var party = await _partyRepo.GetByIdAsync(command.CustomerId.Value, ct)
                ?? throw new KeyNotFoundException($"Party {command.CustomerId.Value} not found.");
            customerName = party.Name;
        }

        using var conn = _factory.Create();
        var orderNumber = await conn.QuerySingleAsync<int>("SELECT NEXT VALUE FOR mdata.seq_order_number");

        var uom = article.UomAbbreviation ?? command.UnitOfMeasure;

        var order = new ProductionOrder(
            orderNumber, article.Id, article.Code, article.Name, article.Revision,
            command.CustomerId, customerName, command.Quantity, uom,
            command.DueDate, command.Notes, command.QuoteId);

        var orderBom = bom.Select(b => new OrderBomLine(
            order.Id, b.ChildArticleId, b.ChildCode, b.ChildName,
            b.Quantity, b.UnitOfMeasureAbbreviation ?? uom, null)).ToList();

        var orderOps = ops.Select(o => new OrderOperation(
            order.Id, o.SequenceNumber, o.OperationTypeId, o.OperationTypeName,
            o.IsSubcontracted, o.EstimatedMinutes, o.Notes, o.IsConditional)).ToList();

        await _orderRepo.SaveAsync(order, orderBom, orderOps, ct);

        await _bus.Publish(new OrderCreatedEvent(
            order.Id, order.OrderNumber, article.Id, article.Code, article.Name,
            command.CustomerId, customerName, command.Quantity, uom,
            command.DueDate, DateTime.UtcNow), ct);

        return order.Id;
    }
}

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IBus _bus;

    public UpdateOrderStatusHandler(IOrderRepository orderRepo, IBus bus)
    {
        _orderRepo = orderRepo;
        _bus = bus;
    }

    public async Task Handle(UpdateOrderStatusCommand command, CancellationToken ct)
    {
        if (!OrderStatus.IsValid(command.NewStatus))
            throw new InvalidOperationException($"Invalid status: {command.NewStatus}.");

        var order = await _orderRepo.GetByIdAsync(command.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {command.OrderId} not found.");

        var oldStatus = order.Status;
        order.TransitionTo(command.NewStatus);
        await _orderRepo.UpdateStatusAsync(order, ct);

        await _bus.Publish(new OrderStatusChangedEvent(
            order.Id, order.OrderNumber, oldStatus, command.NewStatus, DateTime.UtcNow), ct);
    }
}

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IBus _bus;

    public CancelOrderHandler(IOrderRepository orderRepo, IBus bus)
    {
        _orderRepo = orderRepo;
        _bus = bus;
    }

    public async Task Handle(CancelOrderCommand command, CancellationToken ct)
    {
        var order = await _orderRepo.GetByIdAsync(command.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {command.OrderId} not found.");

        var oldStatus = order.Status;
        order.TransitionTo(OrderStatus.Cancelled);
        await _orderRepo.UpdateStatusAsync(order, ct);

        await _bus.Publish(new OrderStatusChangedEvent(
            order.Id, order.OrderNumber, oldStatus, OrderStatus.Cancelled, DateTime.UtcNow), ct);
    }
}
