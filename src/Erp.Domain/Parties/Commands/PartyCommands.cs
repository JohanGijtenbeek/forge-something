using MediatR;

namespace Erp.Domain.Parties.Commands;

// Commands zijn de intent — wat wil de gebruiker doen?
// IRequest<T> = MediatR command met een return type

public record CreateOrganizationCommand(
    string Name,
    string? VatNumber,
    string? ChamberOfCommerceNumber,
    bool RegisterAsCustomer,
    bool RegisterAsSupplier
) : IRequest<Guid>;

public record CreatePersonCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Initials
) : IRequest<Guid>;

public record DeactivatePartyCommand(
    Guid PartyId
) : IRequest;

public record UpdateOrganizationCommand(
    Guid PartyId,
    string Name,
    string? VatNumber,
    string? ChamberOfCommerceNumber
) : IRequest;

public record UpdatePersonCommand(
    Guid PartyId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Initials
) : IRequest;
