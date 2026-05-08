using Erp.Domain.Parties;

namespace Erp.Api.Endpoints;

// ============================================================
// RESPONSES
// ============================================================

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record PartyListResponse(
    Guid Id,
    string Name,
    string PartyType,
    bool IsActive,
    bool IsCustomer,
    bool IsSupplier,
    string? City
);

public record PartyDetailResponse(
    Guid Id,
    string Name,
    string PartyType,
    bool IsActive,
    bool IsCustomer,
    bool IsSupplier,
    PersonDetailsResponse? PersonDetails,
    OrganizationDetailsResponse? OrganizationDetails,
    CustomerRoleResponse? CustomerRole,
    SupplierRoleResponse? SupplierRole,
    IEnumerable<AddressResponse> Addresses,
    IEnumerable<ContactMethodResponse> ContactMethods,
    IEnumerable<BankAccountResponse> BankAccounts
);

public record PersonDetailsResponse(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Initials,
    string FullName
);

public record OrganizationDetailsResponse(
    string? VatNumber,
    string? ChamberOfCommerceNumber
);

public record CustomerRoleResponse(
    int DebtorNumber,
    decimal Discount,
    bool IsVatShifted,
    short PaymentTermDays,
    decimal? CreditLimit
);

public record SupplierRoleResponse(
    int SupplierNumber,
    bool IsVatShifted,
    short PaymentTermDays
);

public record AddressResponse(
    string AddressType,
    string Street,
    string HouseNumber,
    string? HouseNumberAddition,
    string PostalCode,
    string City,
    string CountryCode,
    string? Attention,
    bool IsDefault
);

public record ContactMethodResponse(
    string ContactMethodType,
    string Value,
    bool IsPrimary
);

public record BankAccountResponse(
    string Iban,
    string? Bic,
    string? AccountHolder,
    bool IsPrimary
);

// ============================================================
// REQUESTS
// ============================================================

public record CreateOrganizationRequest(
    string Name,
    string? VatNumber,
    string? ChamberOfCommerceNumber,
    bool RegisterAsCustomer,
    bool RegisterAsSupplier
);

public record CreatePersonRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Initials
);

// ============================================================
// MAPPERS
// ============================================================

public static class PartyMapper
{
    public static PartyListResponse ToListResponse(Party party) => new(
        party.Id,
        party.Name,
        party.PartyType.ToString(),
        party.IsActive,
        party.IsCustomer,
        party.IsSupplier,
        party.Addresses.FirstOrDefault(a => a.IsDefault)?.City
    );

    public static PartyDetailResponse ToDetailResponse(Party party) => new(
        party.Id,
        party.Name,
        party.PartyType.ToString(),
        party.IsActive,
        party.IsCustomer,
        party.IsSupplier,
        party.PersonDetails is null ? null : new PersonDetailsResponse(
            party.PersonDetails.FirstName,
            party.PersonDetails.MiddleName,
            party.PersonDetails.LastName,
            party.PersonDetails.Initials,
            party.PersonDetails.FullName
        ),
        party.OrganizationDetails is null ? null : new OrganizationDetailsResponse(
            party.OrganizationDetails.VatNumber,
            party.OrganizationDetails.ChamberOfCommerceNumber
        ),
        party.CustomerRole is null ? null : new CustomerRoleResponse(
            party.CustomerRole.DebtorNumber,
            party.CustomerRole.Discount,
            party.CustomerRole.IsVatShifted,
            party.CustomerRole.PaymentTermDays,
            party.CustomerRole.CreditLimit
        ),
        party.SupplierRole is null ? null : new SupplierRoleResponse(
            party.SupplierRole.SupplierNumber,
            party.SupplierRole.IsVatShifted,
            party.SupplierRole.PaymentTermDays
        ),
        party.Addresses.Select(a => new AddressResponse(
            a.AddressType.ToString(),
            a.Street,
            a.HouseNumber,
            a.HouseNumberAddition,
            a.PostalCode,
            a.City,
            a.CountryCode,
            a.Attention,
            a.IsDefault
        )),
        party.ContactMethods.Select(c => new ContactMethodResponse(
            c.ContactMethodType.ToString(),
            c.Value,
            c.IsPrimary
        )),
        party.BankAccounts.Select(b => new BankAccountResponse(
            b.Iban,
            b.Bic,
            b.AccountHolder,
            b.IsPrimary
        ))
    );
}

// Update requests
public record UpdateOrganizationRequest(
    string Name,
    string? VatNumber,
    string? ChamberOfCommerceNumber
);

public record UpdatePersonRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Initials
);

// History response
public record PartyHistoryEntryResponse(
    long Id,
    string EventType,
    string Summary,
    string ChangedBy,
    DateTime ChangedAt
);
