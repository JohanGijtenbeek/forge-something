using Erp.Domain.Articles;
using Erp.Domain.Articles.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Erp.Api.Endpoints;

public static class ArticleEndpoints
{
    public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/articles")
            .WithTags("Articles")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        // ── Article CRUD ──────────────────────────────────────────────────

        group.MapGet("/", async Task<Ok<PagedResult<ArticleListResponse>>> (
                IArticleRepository repo, int page = 1, int pageSize = 25, bool includeInactive = false,
                string? search = null, Guid? categoryId = null, string? articleType = null,
                CancellationToken ct = default) =>
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100);
                var (items, totalCount) = await repo.GetPagedAsync(page, pageSize, includeInactive, search, categoryId, articleType, ct);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                return TypedResults.Ok(new PagedResult<ArticleListResponse>(
                    items.Select(ArticleMapper.ToListResponse), totalCount, page, pageSize, totalPages));
            })
            .WithName("GetArticles").WithSummary("Alle artikelen ophalen (gepagineerd)");

        group.MapGet("/{id:guid}", async Task<Results<Ok<ArticleDetailResponse>, NotFound>> (
                Guid id, IArticleRepository repo, CancellationToken ct = default) =>
            {
                var article = await repo.GetByIdAsync(id, ct);
                return article is null ? TypedResults.NotFound() : TypedResults.Ok(ArticleMapper.ToDetailResponse(article));
            })
            .WithName("GetArticleById").WithSummary("Artikel ophalen op ID");

        group.MapGet("/{id:guid}/history", async Task<Ok<IEnumerable<ArticleHistoryEntryResponse>>> (
                Guid id, IArticleRepository repo, CancellationToken ct = default) =>
            {
                var history = await repo.GetHistoryAsync(id, ct);
                return TypedResults.Ok(history.Select(h =>
                    new ArticleHistoryEntryResponse(h.Id, h.EventType, h.Summary, h.ChangedBy, h.ChangedAt)));
            })
            .WithName("GetArticleHistory").WithSummary("Wijzigingshistorie ophalen");

        group.MapPost("/", async Task<Results<Created<object>, BadRequest<string>>> (
                CreateArticleRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return TypedResults.BadRequest("Code is verplicht.");
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TypedResults.BadRequest("Naam is verplicht.");
                if (!ArticleType.IsValid(request.ArticleType))
                    return TypedResults.BadRequest($"Ongeldig article type: {request.ArticleType}. Geldige waarden: raw_material, manufactured, bought_out, service.");

                try
                {
                    var id = await mediator.Send(new CreateArticleCommand(
                        request.Code, request.Name, request.ArticleType,
                        request.Description, request.CategoryId, request.UnitOfMeasureId, request.PurchasePrice,
                        request.Revision), ct);

                    return TypedResults.Created($"/api/articles/{id}", (object)new { id });
                }
                catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    return TypedResults.BadRequest("Een artikel met deze code bestaat al.");
                }
            })
            .WithName("CreateArticle").WithSummary("Artikel aanmaken");

        group.MapPut("/{id:guid}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, UpdateArticleRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return TypedResults.BadRequest("Code is verplicht.");
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TypedResults.BadRequest("Naam is verplicht.");
                if (!ArticleType.IsValid(request.ArticleType))
                    return TypedResults.BadRequest($"Ongeldig article type: {request.ArticleType}.");

                try
                {
                    await mediator.Send(new UpdateArticleCommand(
                        id, request.Code, request.Name, request.ArticleType,
                        request.Description, request.CategoryId, request.UnitOfMeasureId, request.PurchasePrice,
                        request.Revision), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
                catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    return TypedResults.BadRequest("Een artikel met deze code bestaat al.");
                }
            })
            .WithName("UpdateArticle").WithSummary("Artikel bijwerken");

        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (
                Guid id, IMediator mediator, CancellationToken ct = default) =>
            {
                try
                {
                    await mediator.Send(new DeactivateArticleCommand(id), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("DeactivateArticle").WithSummary("Artikel deactiveren");

        // ── BOM ──────────────────────────────────────────────────────────

        group.MapGet("/{id:guid}/bom", async Task<Ok<IEnumerable<BomLineResponse>>> (
                Guid id, IArticleRepository repo, CancellationToken ct = default) =>
            {
                var lines = await repo.GetBomAsync(id, ct);
                return TypedResults.Ok(lines.Select(ArticleMapper.ToBomLineResponse));
            })
            .WithName("GetArticleBom").WithSummary("Stuklijst ophalen");

        group.MapPost("/{id:guid}/bom", async Task<Results<Created<object>, NotFound, BadRequest<string>, Conflict>> (
                Guid id, AddBomComponentRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (request.Quantity <= 0)
                    return TypedResults.BadRequest("Hoeveelheid moet groter zijn dan 0.");

                try
                {
                    var lineId = await mediator.Send(new AddBomComponentCommand(
                        id, request.ChildArticleId, request.Quantity, request.UnitOfMeasureId, request.SortOrder), ct);
                    return TypedResults.Created($"/api/articles/{id}/bom/{lineId}", (object)new { id = lineId });
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
                catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    return TypedResults.Conflict();
                }
            })
            .WithName("AddBomComponent").WithSummary("Component toevoegen aan stuklijst");

        group.MapPut("/{id:guid}/bom/{lineId:guid}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, Guid lineId, UpdateBomComponentRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (request.Quantity <= 0)
                    return TypedResults.BadRequest("Hoeveelheid moet groter zijn dan 0.");

                try
                {
                    await mediator.Send(new UpdateBomComponentCommand(
                        lineId, request.Quantity, request.UnitOfMeasureId, request.SortOrder), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("UpdateBomComponent").WithSummary("Component in stuklijst bijwerken");

        group.MapDelete("/{id:guid}/bom/{lineId:guid}", async Task<Results<NoContent, NotFound>> (
                Guid id, Guid lineId, IMediator mediator, CancellationToken ct = default) =>
            {
                try
                {
                    await mediator.Send(new RemoveBomComponentCommand(lineId), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("RemoveBomComponent").WithSummary("Component verwijderen uit stuklijst");

        // ── Operations (routing template) ─────────────────────────────────

        group.MapGet("/{id:guid}/operations", async Task<Ok<IEnumerable<ArticleOperationResponse>>> (
                Guid id, IArticleRepository repo, CancellationToken ct = default) =>
            {
                var ops = await repo.GetOperationsAsync(id, ct);
                return TypedResults.Ok(ops.Select(ArticleMapper.ToOperationResponse));
            })
            .WithName("GetArticleOperations").WithSummary("Bewerkingen ophalen (routetemplate)");

        group.MapPost("/{id:guid}/operations", async Task<Results<Created<object>, BadRequest<string>, NotFound>> (
                Guid id, AddArticleOperationRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (request.SequenceNumber <= 0)
                    return TypedResults.BadRequest("Volgordenummer moet groter zijn dan 0.");

                try
                {
                    var opId = await mediator.Send(new AddArticleOperationCommand(
                        id, request.SequenceNumber, request.OperationTypeId,
                        request.EstimatedMinutes, request.Notes, request.IsConditional), ct);
                    return TypedResults.Created($"/api/articles/{id}/operations/{opId}", (object)new { id = opId });
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
            .WithName("AddArticleOperation").WithSummary("Bewerking toevoegen aan routetemplate");

        group.MapPut("/{id:guid}/operations/{opId:guid}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, Guid opId, UpdateArticleOperationRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (request.SequenceNumber <= 0)
                    return TypedResults.BadRequest("Volgordenummer moet groter zijn dan 0.");

                try
                {
                    await mediator.Send(new UpdateArticleOperationCommand(
                        opId, request.SequenceNumber, request.EstimatedMinutes, request.Notes, request.IsConditional), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("UpdateArticleOperation").WithSummary("Bewerking in routetemplate bijwerken");

        group.MapDelete("/{id:guid}/operations/{opId:guid}", async Task<Results<NoContent, NotFound>> (
                Guid id, Guid opId, IMediator mediator, CancellationToken ct = default) =>
            {
                try
                {
                    await mediator.Send(new RemoveArticleOperationCommand(opId), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("RemoveArticleOperation").WithSummary("Bewerking verwijderen uit routetemplate");

        return app;
    }

    public static IEndpointRouteBuilder MapArticleCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/article-categories")
            .WithTags("Articles")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<IEnumerable<ArticleCategoryResponse>>> (
                IArticleRepository repo, CancellationToken ct = default) =>
            {
                var categories = await repo.GetCategoriesAsync(ct);
                return TypedResults.Ok(categories.Select(c =>
                    new ArticleCategoryResponse(c.Id, c.Name, c.SortOrder, c.IsActive)));
            })
            .WithName("GetArticleCategories").WithSummary("Artikel categorieën ophalen");

        group.MapPost("/", async Task<Results<Created<object>, BadRequest<string>>> (
                CreateArticleCategoryRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TypedResults.BadRequest("Naam is verplicht.");

                try
                {
                    var id = await mediator.Send(new CreateArticleCategoryCommand(request.Name, request.SortOrder), ct);
                    return TypedResults.Created($"/api/article-categories/{id}", (object)new { id });
                }
                catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    return TypedResults.BadRequest("Een categorie met deze naam bestaat al.");
                }
            })
            .WithName("CreateArticleCategory").WithSummary("Artikel categorie aanmaken");

        return app;
    }

    public static IEndpointRouteBuilder MapOperationTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/operation-types")
            .WithTags("Articles")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<IEnumerable<OperationTypeResponse>>> (
                IArticleRepository repo, CancellationToken ct = default) =>
            {
                var types = await repo.GetOperationTypesAsync(ct);
                return TypedResults.Ok(types.Select(ArticleMapper.ToOperationTypeResponse));
            })
            .WithName("GetOperationTypes").WithSummary("Bewerkingstypen ophalen");

        return app;
    }

    public static IEndpointRouteBuilder MapMachineTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/machine-types")
            .WithTags("Articles")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<IEnumerable<MachineTypeResponse>>> (
                IArticleRepository repo, CancellationToken ct = default) =>
            {
                var types = await repo.GetMachineTypesAsync(ct);
                return TypedResults.Ok(types.Select(ArticleMapper.ToMachineTypeResponse));
            })
            .WithName("GetMachineTypes").WithSummary("Machinetypen ophalen");

        return app;
    }

    public static IEndpointRouteBuilder MapUnitOfMeasureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units-of-measure")
            .WithTags("Articles")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<IEnumerable<UnitOfMeasureResponse>>> (
                IArticleRepository repo, CancellationToken ct = default) =>
            {
                var uoms = await repo.GetUnitsOfMeasureAsync(ct);
                return TypedResults.Ok(uoms.Select(u =>
                    new UnitOfMeasureResponse(u.Id, u.Name, u.Abbreviation, u.IsActive)));
            })
            .WithName("GetUnitsOfMeasure").WithSummary("Maateenheden ophalen");

        group.MapPost("/", async Task<Results<Created<object>, BadRequest<string>>> (
                CreateUnitOfMeasureRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TypedResults.BadRequest("Naam is verplicht.");
                if (string.IsNullOrWhiteSpace(request.Abbreviation))
                    return TypedResults.BadRequest("Afkorting is verplicht.");

                try
                {
                    var id = await mediator.Send(new CreateUnitOfMeasureCommand(request.Name, request.Abbreviation), ct);
                    return TypedResults.Created($"/api/units-of-measure/{id}", (object)new { id });
                }
                catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    return TypedResults.BadRequest("Een maateenheid met deze naam bestaat al.");
                }
            })
            .WithName("CreateUnitOfMeasure").WithSummary("Maateenheid aanmaken");

        return app;
    }
}
