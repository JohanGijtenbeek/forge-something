using Bogus;
using Erp.Seeder.Models;

namespace Erp.Seeder.Generators;

public class PartyGenerator
{
    private readonly HashSet<string> _usedNames = new();
    private readonly Faker _faker;
    private readonly Faker _deFaker;
    private readonly Faker _beFaker;

    private static readonly string[] CompanyTypes =
    [
        "Metaal", "Staal", "Techniek", "Industrie", "Constructie",
        "Machinefabriek", "Handel", "Plaatbewerking", "Lasbedrijf",
        "Fabricage", "Groep", "Holding"
    ];

    private static readonly string[] CompanySuffixes =
        ["B.V.", "B.V.", "B.V.", "N.V.", "V.O.F.", "B.V. & Co."];

    private static readonly string[] DutchTussenvoegsels =
        ["de", "van", "van de", "van den", "van der", "den", "ter", "ten", null!, null!, null!, null!];

    private static readonly short[] PaymentTerms = [14, 30, 30, 30, 45, 60, 90];

    public PartyGenerator(int seed)
    {
        Randomizer.Seed = new Random(seed);
        _faker = new Faker("nl");
        _deFaker = new Faker("de");
        _beFaker = new Faker("fr");
    }

    public GeneratedParty GenerateOrganization()
    {
        var id = Guid.NewGuid();
        var country = PickCountry();
        var faker = GetFaker(country);

        var name = GenerateCompanyName(faker);
        var isActive = _faker.Random.Bool(0.9f);

        var party = new PartyRow(id, 1, name, isActive);

        var orgDetail = new OrganizationDetailRow(
            id,
            GenerateVatNumber(country, faker),
            faker.Random.Replace("########")
        );

        var addresses = GenerateAddresses(id, country, faker, null);
        var contactMethods = GenerateContactMethods(id, name, faker);
        var bankAccount = isActive ? GenerateBankAccount(id, name, country) : null;

        // Verdeling: 60% klant, 20% leverancier, 20% beide
        var role = _faker.Random.WeightedRandom(
            new[] { "customer", "supplier", "both", "none" },
            new[] { 0.55f, 0.15f, 0.20f, 0.10f });

        var customerRole = role is "customer" or "both"
            ? GenerateCustomerRole(id, country)
            : null;

        var supplierRole = role is "supplier" or "both"
            ? GenerateSupplierRole(id, country)
            : null;

        return new GeneratedParty(party, null, orgDetail, addresses,
            contactMethods, bankAccount, customerRole, supplierRole);
    }

    public GeneratedParty GeneratePerson()
    {
        var id = Guid.NewGuid();
        var firstName = _faker.Name.FirstName();
        var middleName = _faker.PickRandom(DutchTussenvoegsels);
        var lastName = _faker.Name.LastName();
        var initials = $"{firstName[0]}.";

        var fullName = string.IsNullOrEmpty(middleName)
            ? $"{firstName} {lastName}"
            : $"{firstName} {middleName} {lastName}";

        var party = new PartyRow(id, 2, fullName, true);

        var personDetail = new PersonDetailRow(
            id, firstName, middleName, lastName, initials);

        var addresses = GenerateAddresses(id, "NL", _faker, null);
        var contactMethods = GenerateContactMethods(id, fullName, _faker);

        return new GeneratedParty(party, personDetail, null, addresses,
            contactMethods, null, null, null);
    }

    public List<PartyRelationshipRow> GenerateRelationships(
        List<Guid> orgIds, List<Guid> personIds)
    {
        var relationships = new List<PartyRelationshipRow>();
        var used = new HashSet<(Guid, Guid)>();

        foreach (var personId in personIds)
        {
            // Elke persoon is contactpersoon bij 1-2 organisaties
            var count = _faker.Random.Int(1, 2);
            var orgs = _faker.Random.ListItems(orgIds, count);

            foreach (var orgId in orgs)
            {
                if (used.Add((orgId, personId)))
                    relationships.Add(new PartyRelationshipRow(orgId, personId, 1));
            }
        }

        return relationships;
    }

    // ============================================================
    // Private helpers
    // ============================================================

    private string PickCountry() =>
        _faker.Random.WeightedRandom(
            new[] { "NL", "DE", "BE" },
            new[] { 0.90f, 0.05f, 0.05f });

    private Faker GetFaker(string country) => country switch
    {
        "DE" => _deFaker,
        "BE" => _beFaker,
        _ => _faker
    };

    private string GenerateCompanyName(Faker faker)
    {
        return CompanyNames.GenerateUniqueName(_usedNames);
    }

    private string? GenerateVatNumber(string country, Faker faker) =>
        country switch
        {
            "NL" => $"NL{faker.Random.Number(100000000, 999999999)}B01",
            "DE" => $"DE{faker.Random.Number(100000000, 999999999)}",
            "BE" => $"BE0{faker.Random.Number(100000000, 999999999)}",
            _ => null
        };

    private List<PartyAddressRow> GenerateAddresses(
        Guid partyId, string country, Faker faker, string? attention)
    {
        var addresses = new List<PartyAddressRow>();

        // Postadres
        addresses.Add(new PartyAddressRow(
            partyId, 1,
            faker.Address.StreetName(),
            faker.Random.Number(1, 999).ToString(),
            _faker.Random.Bool(0.2f) ? _faker.Random.AlphaNumeric(1).ToUpper() : null,
            GeneratePostalCode(country, faker),
            faker.Address.City(),
            country,
            attention,
            true
        ));

        // 30% kans op apart afleveradres
        if (_faker.Random.Bool(0.3f))
        {
            addresses.Add(new PartyAddressRow(
                partyId, 2,
                faker.Address.StreetName(),
                faker.Random.Number(1, 999).ToString(),
                null,
                GeneratePostalCode(country, faker),
                faker.Address.City(),
                country,
                null,
                true
            ));
        }

        return addresses;
    }

    private string GeneratePostalCode(string country, Faker faker) =>
        country switch
        {
            "NL" => $"{faker.Random.Number(1000, 9999)} {faker.Random.String2(2, "ABCDEFGHJKLMNPRSTUVWXYZ")}",
            "DE" => faker.Random.Number(10000, 99999).ToString(),
            "BE" => faker.Random.Number(1000, 9999).ToString(),
            _ => faker.Address.ZipCode()
        };

    private List<PartyContactMethodRow> GenerateContactMethods(
        Guid partyId, string name, Faker faker)
    {
        return
        [
            new(partyId, 1, faker.Phone.PhoneNumber("0##########"), true),
            new(partyId, 2,
                faker.Internet.Email(name.Split(' ')[0].ToLower(),
                    name.Split(' ').Last().ToLower()),
                true),
        ];
    }

    private PartyBankAccountRow GenerateBankAccount(
        Guid partyId, string name, string country)
    {
        var iban = country == "NL"
            ? $"NL{_faker.Random.Number(10, 99)}INGB{_faker.Random.Number(100000000, 999999999)}{_faker.Random.Number(10, 99)}"
            : $"{country}{_faker.Random.Number(10, 99)}{_faker.Random.AlphaNumeric(4).ToUpper()}{_faker.Random.Number(10000000, 99999999)}";

        return new PartyBankAccountRow(partyId, iban, "INGBNL2A", name, true);
    }

    private CustomerRoleRow GenerateCustomerRole(Guid partyId, string country)
    {
        var isVatShifted = country != "NL";
        var discount = _faker.Random.WeightedRandom(
            new[] { 0m, 5m, 10m, 15m, 20m, 25m },
            new[] { 0.5f, 0.2f, 0.15f, 0.08f, 0.05f, 0.02f });
        var paymentTerm = _faker.PickRandom(PaymentTerms);
        var creditLimit = _faker.Random.Bool(0.8f)
            ? (decimal?)_faker.Random.Number(5000, 100000)
            : null;

        return new CustomerRoleRow(partyId, discount, isVatShifted, paymentTerm, creditLimit);
    }

    private SupplierRoleRow GenerateSupplierRole(Guid partyId, string country)
    {
        var isVatShifted = country != "NL";
        var paymentTerm = _faker.PickRandom(PaymentTerms);
        return new SupplierRoleRow(partyId, isVatShifted, paymentTerm);
    }
}
