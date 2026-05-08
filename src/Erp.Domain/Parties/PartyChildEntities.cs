namespace Erp.Domain.Parties;

public partial class PartyAddress
{
    public Guid Id { get; private set; }
    public Guid PartyId { get; private set; }
    public AddressType AddressType { get; private set; }
    public string Street { get; private set; } = string.Empty;
    public string HouseNumber { get; private set; } = string.Empty;
    public string? HouseNumberAddition { get; private set; }
    public string PostalCode { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public string? Attention { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PartyAddress() { }

    public PartyAddress(
        Guid partyId,
        AddressType addressType,
        string street,
        string houseNumber,
        string? houseNumberAddition,
        string postalCode,
        string city,
        string countryCode = "NL",
        string? attention = null,
        bool isDefault = false)
    {
        Id = Guid.NewGuid();
        PartyId = partyId;
        AddressType = addressType;
        Street = street;
        HouseNumber = houseNumber;
        HouseNumberAddition = houseNumberAddition;
        PostalCode = postalCode;
        City = city;
        CountryCode = countryCode;
        Attention = attention;
        IsDefault = isDefault;
        CreatedAt = DateTime.UtcNow;
    }
}

public partial class PartyContactMethod
{
    public Guid Id { get; private set; }
    public Guid PartyId { get; private set; }
    public ContactMethodType ContactMethodType { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PartyContactMethod() { }

    public PartyContactMethod(Guid partyId, ContactMethodType type, string value, bool isPrimary = false)
    {
        Id = Guid.NewGuid();
        PartyId = partyId;
        ContactMethodType = type;
        Value = value;
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;
    }
}

public partial class PartyBankAccount
{
    public Guid Id { get; private set; }
    public Guid PartyId { get; private set; }
    public string Iban { get; private set; } = string.Empty;
    public string? Bic { get; private set; }
    public string? AccountHolder { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PartyBankAccount() { }

    public PartyBankAccount(Guid partyId, string iban, string? bic, string? accountHolder, bool isPrimary = false)
    {
        Id = Guid.NewGuid();
        PartyId = partyId;
        Iban = iban;
        Bic = bic;
        AccountHolder = accountHolder;
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;
    }
}

public class PartyRelationship
{
    public Guid Id { get; private set; }
    public Guid FromPartyId { get; private set; }
    public Guid ToPartyId { get; private set; }
    public PartyRelationshipType RelationshipType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PartyRelationship(Guid fromPartyId, Guid toPartyId, PartyRelationshipType relationshipType)
    {
        Id = Guid.NewGuid();
        FromPartyId = fromPartyId;
        ToPartyId = toPartyId;
        RelationshipType = relationshipType;
        CreatedAt = DateTime.UtcNow;
    }
}

public partial class PersonDetails
{
    public Guid PartyId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }   // tussenvoegsel
    public string LastName { get; private set; } = string.Empty;
    public string? Initials { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Computed - handig voor weergave en correspondentie
    public string FullName => string.IsNullOrEmpty(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    public string FormalName => string.IsNullOrEmpty(MiddleName)
        ? $"{LastName}, {FirstName}"
        : $"{LastName}, {FirstName} {MiddleName}";

    private PersonDetails() { }

    internal PersonDetails(Guid partyId, string firstName, string? middleName, string lastName, string? initials)
    {
        PartyId = partyId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Initials = initials ?? GenerateInitials(firstName, middleName);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string firstName, string? middleName, string lastName, string? initials)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Initials = initials ?? GenerateInitials(firstName, middleName);
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateInitials(string firstName, string? middleName)
    {
        var initials = $"{firstName[0]}.";
        if (!string.IsNullOrEmpty(middleName))
            initials += string.Concat(middleName.Split(' ').Select(w => $"{w[0]}."));
        return initials;
    }
}

public partial class OrganizationDetails
{
    public Guid PartyId { get; private set; }
    public string? VatNumber { get; private set; }
    public string? ChamberOfCommerceNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private OrganizationDetails() { }

    internal OrganizationDetails(Guid partyId, string? vatNumber, string? chamberOfCommerceNumber)
    {
        PartyId = partyId;
        VatNumber = vatNumber;
        ChamberOfCommerceNumber = chamberOfCommerceNumber;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string? vatNumber, string? chamberOfCommerceNumber)
    {
        VatNumber = vatNumber;
        ChamberOfCommerceNumber = chamberOfCommerceNumber;
        UpdatedAt = DateTime.UtcNow;
    }
}

public partial class PersonDetails
{
    public static PersonDetails Reconstitute(Guid partyId, string firstName, string? middleName,
        string lastName, string? initials, DateTime createdAt, DateTime updatedAt) =>
        new() { PartyId = partyId, FirstName = firstName, MiddleName = middleName,
                LastName = lastName, Initials = initials, CreatedAt = createdAt, UpdatedAt = updatedAt };
}

public partial class OrganizationDetails
{
    public static OrganizationDetails Reconstitute(Guid partyId, string? vatNumber,
        string? chamberOfCommerceNumber, DateTime createdAt, DateTime updatedAt) =>
        new() { PartyId = partyId, VatNumber = vatNumber,
                ChamberOfCommerceNumber = chamberOfCommerceNumber, CreatedAt = createdAt, UpdatedAt = updatedAt };
}

public partial class PartyAddress
{
    public static PartyAddress Reconstitute(Guid id, Guid partyId, AddressType addressType,
        string street, string houseNumber, string? houseNumberAddition, string postalCode,
        string city, string countryCode, string? attention, bool isDefault, DateTime createdAt) =>
        new() { Id = id, PartyId = partyId, AddressType = addressType, Street = street,
                HouseNumber = houseNumber, HouseNumberAddition = houseNumberAddition,
                PostalCode = postalCode, City = city, CountryCode = countryCode,
                Attention = attention, IsDefault = isDefault, CreatedAt = createdAt };
}

public partial class PartyContactMethod
{
    public static PartyContactMethod Reconstitute(Guid id, Guid partyId,
        ContactMethodType contactMethodType, string value, bool isPrimary, DateTime createdAt) =>
        new() { Id = id, PartyId = partyId, ContactMethodType = contactMethodType,
                Value = value, IsPrimary = isPrimary, CreatedAt = createdAt };
}

public partial class PartyBankAccount
{
    public static PartyBankAccount Reconstitute(Guid id, Guid partyId, string iban,
        string? bic, string? accountHolder, bool isPrimary, DateTime createdAt) =>
        new() { Id = id, PartyId = partyId, Iban = iban, Bic = bic,
                AccountHolder = accountHolder, IsPrimary = isPrimary, CreatedAt = createdAt };
}
