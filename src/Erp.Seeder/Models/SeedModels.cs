namespace Erp.Seeder.Models;

// Platte modellen voor directe database inserts via Dapper
// Bewust gescheiden van het domeinmodel

public record PartyRow(
    Guid Id,
    int PartyTypeId,
    string Name,
    bool IsActive
);

public record PersonDetailRow(
    Guid PartyId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? Initials
);

public record OrganizationDetailRow(
    Guid PartyId,
    string? VatNumber,
    string? ChamberOfCommerceNumber
);

public record PartyAddressRow(
    Guid PartyId,
    int AddressTypeId,
    string Street,
    string HouseNumber,
    string? HouseNumberAddition,
    string PostalCode,
    string City,
    string CountryCode,
    string? Attention,
    bool IsDefault
);

public record PartyContactMethodRow(
    Guid PartyId,
    int ContactMethodTypeId,
    string Value,
    bool IsPrimary
);

public record PartyBankAccountRow(
    Guid PartyId,
    string Iban,
    string? Bic,
    string? AccountHolder,
    bool IsPrimary
);

public record CustomerRoleRow(
    Guid PartyId,
    decimal Discount,
    bool IsVatShifted,
    short PaymentTermDays,
    decimal? CreditLimit
);

public record SupplierRoleRow(
    Guid PartyId,
    bool IsVatShifted,
    short PaymentTermDays
);

public record PartyRelationshipRow(
    Guid FromPartyId,
    Guid ToPartyId,
    int RelationshipTypeId
);

// Gecombineerd resultaat van de generator
public record GeneratedParty(
    PartyRow Party,
    PersonDetailRow? PersonDetail,
    OrganizationDetailRow? OrganizationDetail,
    List<PartyAddressRow> Addresses,
    List<PartyContactMethodRow> ContactMethods,
    PartyBankAccountRow? BankAccount,
    CustomerRoleRow? CustomerRole,
    SupplierRoleRow? SupplierRole
);

// ── Articles ────────────────────────────────────────────────────────────────

public record ArticleSeedRow(
    Guid Id,
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    Guid? UnitOfMeasureId,
    decimal? PurchasePrice,
    bool IsActive
);

public record ArticleCategorySeedRow(
    Guid Id,
    string Name,
    int SortOrder
);

public record UnitOfMeasureSeedRow(
    Guid Id,
    string Name,
    string Abbreviation
);
