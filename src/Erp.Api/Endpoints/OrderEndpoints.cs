using Erp.Domain.Orders;
using Erp.Domain.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Erp.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<PagedResult<OrderSummaryResponse>>> (
                IOrderRepository repo,
                int page = 1, int pageSize = 25,
                string? search = null, string? status = null,
                CancellationToken ct = default) =>
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100);
                var (items, total) = await repo.GetPagedAsync(page, pageSize, search, status, ct);
                var totalPages = (int)Math.Ceiling(total / (double)pageSize);
                return TypedResults.Ok(new PagedResult<OrderSummaryResponse>(
                    items.Select(OrderMapper.ToSummaryResponse), total, page, pageSize, totalPages));
            })
            .WithName("GetOrders").WithSummary("Production orders ophalen (gepagineerd)");

        group.MapGet("/{id:guid}", async Task<Results<Ok<OrderDetailResponse>, NotFound>> (
                Guid id, IOrderRepository repo, CancellationToken ct = default) =>
            {
                var order = await repo.GetByIdAsync(id, ct);
                if (order is null) return TypedResults.NotFound();

                var bom = await repo.GetBomLinesAsync(id, ct);
                var ops = await repo.GetOperationsAsync(id, ct);
                return TypedResults.Ok(OrderMapper.ToDetailResponse(order, bom, ops));
            })
            .WithName("GetOrderById").WithSummary("Production order ophalen op ID");

        group.MapGet("/{id:guid}/history", async Task<Ok<IEnumerable<OrderHistoryEntryResponse>>> (
                Guid id, IOrderRepository repo, CancellationToken ct = default) =>
            {
                var history = await repo.GetHistoryAsync(id, ct);
                return TypedResults.Ok(history.Select(h =>
                    new OrderHistoryEntryResponse(h.Id, h.EventType, h.Summary, h.ChangedBy, h.ChangedAt)));
            })
            .WithName("GetOrderHistory").WithSummary("Wijzigingshistorie ophalen");

        group.MapPost("/", async Task<Results<Created<object>, BadRequest<string>, NotFound>> (
                CreateOrderRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (request.Quantity <= 0)
                    return TypedResults.BadRequest("Quantity must be greater than 0.");
                if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
                    return TypedResults.BadRequest("Unit of measure is required.");

                try
                {
                    var id = await mediator.Send(new CreateProductionOrderCommand(
                        request.ArticleId, request.CustomerId, request.Quantity,
                        request.UnitOfMeasure, request.DueDate, request.Notes), ct);
                    return TypedResults.Created($"/api/orders/{id}", (object)new { id });
                }
                catch (KeyNotFoundException ex)
                {
                    return TypedResults.NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            })
            .WithName("CreateOrder").WithSummary("Production order aanmaken");

        group.MapPut("/{id:guid}/status", async Task<Results<NoContent, BadRequest<string>, NotFound>> (
                Guid id, UpdateOrderStatusRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Status))
                    return TypedResults.BadRequest("Status is required.");

                try
                {
                    await mediator.Send(new UpdateOrderStatusCommand(id, request.Status.ToLower()), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            })
            .WithName("UpdateOrderStatus").WithSummary("Status van production order wijzigen");

        group.MapDelete("/{id:guid}", async Task<Results<NoContent, BadRequest<string>, NotFound>> (
                Guid id, IMediator mediator, CancellationToken ct = default) =>
            {
                try
                {
                    await mediator.Send(new CancelOrderCommand(id), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            })
            .WithName("CancelOrder").WithSummary("Production order annuleren");

        return app;
    }
}
