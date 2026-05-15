using Erp.Domain.Quotes;
using Erp.Domain.Quotes.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Erp.Api.Endpoints;

public static class QuoteEndpoints
{
    public static IEndpointRouteBuilder MapQuoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/quotes")
            .WithTags("Quotes")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<PagedResult<QuoteSummaryResponse>>> (
                IQuoteRepository repo,
                int page = 1, int pageSize = 25,
                string? search = null, string? status = null) =>
            {
                page     = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100);
                var (items, total) = await repo.GetPagedAsync(page, pageSize, search, status);
                var totalPages = (int)Math.Ceiling(total / (double)pageSize);

                var responses = new List<QuoteSummaryResponse>();
                foreach (var q in items)
                {
                    var lines = await repo.GetLinesAsync(q.Id);
                    responses.Add(new QuoteSummaryResponse(
                        q.Id, q.QuoteNumber, q.CustomerName, q.Date,
                        q.Status, lines.Count(), q.CreatedAt));
                }

                return TypedResults.Ok(new PagedResult<QuoteSummaryResponse>(
                    responses, total, page, pageSize, totalPages));
            })
            .WithName("GetQuotes").WithSummary("Offertes ophalen (gepagineerd)");

        group.MapGet("/{id:guid}", async Task<Results<Ok<QuoteDetailResponse>, NotFound>> (
                Guid id, IQuoteRepository repo) =>
            {
                var quote = await repo.GetByIdAsync(id);
                if (quote is null) return TypedResults.NotFound();

                var lines = await repo.GetLinesAsync(id);
                return TypedResults.Ok(QuoteMapper.ToDetailResponse(quote, lines));
            })
            .WithName("GetQuoteById").WithSummary("Offerte ophalen op ID");

        group.MapGet("/{id:guid}/history", async Task<Ok<IEnumerable<QuoteHistoryEntryResponse>>> (
                Guid id, IQuoteRepository repo) =>
            {
                var history = await repo.GetHistoryAsync(id);
                return TypedResults.Ok(history.Select(h =>
                    new QuoteHistoryEntryResponse(h.Id, h.EventType, h.Summary, h.ChangedBy, h.ChangedAt)));
            })
            .WithName("GetQuoteHistory").WithSummary("Wijzigingshistorie offerte ophalen");

        group.MapPost("/", async Task<Results<Created<object>, BadRequest<string>>> (
                CreateQuoteRequest request, IMediator mediator) =>
            {
                if (request.HourlyRate <= 0)
                    return TypedResults.BadRequest("Hourly rate must be greater than 0.");
                if (request.MaterialMargin <= 0)
                    return TypedResults.BadRequest("Material margin must be greater than 0.");

                var id = await mediator.Send(new CreateQuoteCommand(
                    request.CustomerId, request.Date, request.Reference,
                    request.ContactPerson, request.DeliveryTime,
                    request.HourlyRate, request.MaterialMargin,
                    request.StandardMargin, request.SetupTime));

                return TypedResults.Created($"/api/quotes/{id}", (object)new { id });
            })
            .WithName("CreateQuote").WithSummary("Nieuwe offerte aanmaken");

        group.MapPut("/{id:guid}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, UpdateQuoteHeaderRequest request, IMediator mediator) =>
            {
                if (request.HourlyRate <= 0)
                    return TypedResults.BadRequest("Hourly rate must be greater than 0.");

                try
                {
                    await mediator.Send(new UpdateQuoteHeaderCommand(
                        id, request.CustomerId, request.Date, request.Reference,
                        request.ContactPerson, request.DeliveryTime,
                        request.HourlyRate, request.MaterialMargin,
                        request.StandardMargin, request.SetupTime, request.Remarks));
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
            .WithName("UpdateQuoteHeader").WithSummary("Offerte header bijwerken");

        group.MapPut("/{id:guid}/status", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, UpdateQuoteStatusRequest request, IMediator mediator) =>
            {
                if (string.IsNullOrWhiteSpace(request.Status))
                    return TypedResults.BadRequest("Status is required.");

                try
                {
                    await mediator.Send(new UpdateQuoteStatusCommand(id, request.Status.ToLower()));
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
            .WithName("UpdateQuoteStatus").WithSummary("Status van offerte wijzigen");

        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, IMediator mediator) =>
            {
                try
                {
                    await mediator.Send(new UpdateQuoteStatusCommand(id, QuoteStatus.Rejected));
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
            .WithName("DeleteQuote").WithSummary("Offerte afwijzen (soft delete)");

        group.MapPost("/{id:guid}/lines", async Task<Results<Created<object>, NotFound, BadRequest<string>>> (
                Guid id, AddQuoteLineRequest request, IMediator mediator) =>
            {
                if (string.IsNullOrWhiteSpace(request.PartName))
                    return TypedResults.BadRequest("Part name is required.");
                if (request.Quantity <= 0)
                    return TypedResults.BadRequest("Quantity must be greater than 0.");

                try
                {
                    var lineId = await mediator.Send(new AddQuoteLineCommand(
                        id, request.SortOrder, request.PartName, request.PartNumber,
                        request.Quantity, request.ArticleId,
                        request.MaterialType, request.MaterialCode, request.MaterialCode2,
                        request.MaterialGeometry, request.MaterialSizeMm, request.MaterialLengthMm,
                        request.MaterialQuantity, request.MaterialPrice, request.MaterialSource,
                        request.OperationCount, request.OperationTimeMinutes,
                        request.SubcontractingCount, request.SubcontractingPrice,
                        request.IsManualPrice, request.ManualPrice, request.Remarks));

                    return TypedResults.Created($"/api/quotes/{id}/lines/{lineId}", (object)new { id = lineId });
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
            .WithName("AddQuoteLine").WithSummary("Offerte regel toevoegen");

        group.MapPut("/{id:guid}/lines/{lineId:guid}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, Guid lineId, UpdateQuoteLineRequest request, IMediator mediator) =>
            {
                if (string.IsNullOrWhiteSpace(request.PartName))
                    return TypedResults.BadRequest("Part name is required.");
                if (request.Quantity <= 0)
                    return TypedResults.BadRequest("Quantity must be greater than 0.");

                try
                {
                    await mediator.Send(new UpdateQuoteLineCommand(
                        lineId, request.SortOrder, request.PartName, request.PartNumber,
                        request.Quantity, request.ArticleId,
                        request.MaterialType, request.MaterialCode, request.MaterialCode2,
                        request.MaterialGeometry, request.MaterialSizeMm, request.MaterialLengthMm,
                        request.MaterialQuantity, request.MaterialPrice, request.MaterialSource,
                        request.OperationCount, request.OperationTimeMinutes,
                        request.SubcontractingCount, request.SubcontractingPrice,
                        request.IsManualPrice, request.ManualPrice, request.Remarks));
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
            .WithName("UpdateQuoteLine").WithSummary("Offerte regel bijwerken");

        group.MapDelete("/{id:guid}/lines/{lineId:guid}", async Task<Results<NoContent, NotFound>> (
                Guid id, Guid lineId, IMediator mediator) =>
            {
                try
                {
                    await mediator.Send(new RemoveQuoteLineCommand(lineId));
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("RemoveQuoteLine").WithSummary("Offerte regel verwijderen");

        group.MapPut("/{id:guid}/lines/{lineId:guid}/accept", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, Guid lineId, IMediator mediator) =>
            {
                try
                {
                    await mediator.Send(new AcceptQuoteLineCommand(lineId));
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
            .WithName("AcceptQuoteLine").WithSummary("Offerte regel accepteren");

        group.MapPost("/{id:guid}/convert", async Task<Results<Ok<ConvertQuoteResponse>, NotFound, BadRequest<string>>> (
                Guid id, IMediator mediator) =>
            {
                try
                {
                    var orderIds = await mediator.Send(new ConvertQuoteToOrdersCommand(id));
                    return TypedResults.Ok(new ConvertQuoteResponse(id, orderIds));
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
            .WithName("ConvertQuoteToOrders").WithSummary("Offerte omzetten naar productieorders");

        return app;
    }
}
