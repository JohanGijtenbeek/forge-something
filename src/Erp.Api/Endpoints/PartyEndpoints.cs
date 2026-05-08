using Erp.Domain.Parties;
using Erp.Domain.Parties.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Erp.Api.Endpoints;

public static class PartyEndpoints
{
    public static IEndpointRouteBuilder MapPartyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/parties")
            .WithTags("Parties")
            .RequireRateLimiting("sliding")
            .RequireRateLimiting("concurrency");

        group.MapGet("/", async Task<Ok<PagedResult<PartyListResponse>>> (
                IPartyRepository repo, int page = 1, int pageSize = 25, bool includeInactive = false, CancellationToken ct = default) =>
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100);
                var (items, totalCount) = await repo.GetPagedAsync(page, pageSize, includeInactive, ct);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                return TypedResults.Ok(new PagedResult<PartyListResponse>(items.Select(PartyMapper.ToListResponse), totalCount, page, pageSize, totalPages));
            })
            .WithName("GetParties").WithSummary("Alle parties ophalen (gepagineerd)");

        group.MapGet("/customers", async Task<Ok<IEnumerable<PartyListResponse>>> (
                IPartyRepository repo, bool includeInactive = false, CancellationToken ct = default) =>
            {
                var parties = await repo.GetCustomersAsync(includeInactive, ct);
                return TypedResults.Ok(parties.Select(PartyMapper.ToListResponse));
            })
            .WithName("GetCustomers").WithSummary("Alle klanten ophalen");

        group.MapGet("/suppliers", async Task<Ok<IEnumerable<PartyListResponse>>> (
                IPartyRepository repo, bool includeInactive = false, CancellationToken ct = default) =>
            {
                var parties = await repo.GetSuppliersAsync(includeInactive, ct);
                return TypedResults.Ok(parties.Select(PartyMapper.ToListResponse));
            })
            .WithName("GetSuppliers").WithSummary("Alle leveranciers ophalen");

        group.MapGet("/{id:guid}", async Task<Results<Ok<PartyDetailResponse>, NotFound>> (
                Guid id, IPartyRepository repo, CancellationToken ct = default) =>
            {
                var party = await repo.GetByIdWithDetailsAsync(id, ct);
                return party is null ? TypedResults.NotFound() : TypedResults.Ok(PartyMapper.ToDetailResponse(party));
            })
            .WithName("GetPartyById").WithSummary("Party ophalen op ID");

        group.MapGet("/{id:guid}/history", async Task<Results<Ok<IEnumerable<PartyHistoryEntryResponse>>, NotFound>> (
                Guid id, IPartyRepository repo, CancellationToken ct = default) =>
            {
                var history = await repo.GetHistoryAsync(id, ct);
                return TypedResults.Ok(history.Select(h => new PartyHistoryEntryResponse(
                    h.Id, h.EventType, h.Summary, h.ChangedBy, h.ChangedAt)));
            })
            .WithName("GetPartyHistory").WithSummary("Wijzigingshistorie ophalen");

        group.MapPost("/organizations", async Task<Results<Created<object>, BadRequest<string>>> (
                CreateOrganizationRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TypedResults.BadRequest("Naam is verplicht.");

                var id = await mediator.Send(new CreateOrganizationCommand(
                    request.Name, request.VatNumber, request.ChamberOfCommerceNumber,
                    request.RegisterAsCustomer, request.RegisterAsSupplier), ct);

                return TypedResults.Created($"/api/parties/{id}", (object)new { id });
            })
            .WithName("CreateOrganization").WithSummary("Organisatie aanmaken");

        group.MapPost("/persons", async Task<Results<Created<object>, BadRequest<string>>> (
                CreatePersonRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                    return TypedResults.BadRequest("Voornaam en achternaam zijn verplicht.");

                var id = await mediator.Send(new CreatePersonCommand(
                    request.FirstName, request.MiddleName, request.LastName, request.Initials), ct);

                return TypedResults.Created($"/api/parties/{id}", (object)new { id });
            })
            .WithName("CreatePerson").WithSummary("Persoon aanmaken");

        group.MapPut("/{id:guid}/organization", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, UpdateOrganizationRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return TypedResults.BadRequest("Naam is verplicht.");
                try
                {
                    await mediator.Send(new UpdateOrganizationCommand(
                        id, request.Name, request.VatNumber, request.ChamberOfCommerceNumber), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("UpdateOrganization").WithSummary("Organisatie bijwerken");

        group.MapPut("/{id:guid}/person", async Task<Results<NoContent, NotFound, BadRequest<string>>> (
                Guid id, UpdatePersonRequest request, IMediator mediator, CancellationToken ct = default) =>
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                    return TypedResults.BadRequest("Voornaam en achternaam zijn verplicht.");
                try
                {
                    await mediator.Send(new UpdatePersonCommand(
                        id, request.FirstName, request.MiddleName, request.LastName, request.Initials), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("UpdatePerson").WithSummary("Persoon bijwerken");

        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (
                Guid id, IMediator mediator, CancellationToken ct = default) =>
            {
                try
                {
                    await mediator.Send(new DeactivatePartyCommand(id), ct);
                    return TypedResults.NoContent();
                }
                catch (KeyNotFoundException)
                {
                    return TypedResults.NotFound();
                }
            })
            .WithName("DeactivateParty").WithSummary("Party deactiveren");

        return app;
    }
}
