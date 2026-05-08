namespace Erp.Domain.Parties;

public partial class Party
{
    public Guid Id { get; private set; }
    public PartyType PartyType { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<PartyAddress> Addresses => _addresses.AsReadOnly();
    public IReadOnlyCollection<PartyContactMethod> ContactMethods => _contactMethods.AsReadOnly();
    public IReadOnlyCollection<PartyBankAccount> BankAccounts => _bankAccounts.AsReadOnly();
    public IReadOnlyCollection<PartyRelationship> Relationships => _relationships.AsReadOnly();
    public PersonDetails? PersonDetails { get; private set; }
    public OrganizationDetails? OrganizationDetails { get; private set; }
    public CustomerRole? CustomerRole { get; private set; }
    public SupplierRole? SupplierRole { get; private set; }

    private readonly List<PartyAddress> _addresses = [];
    private readonly List<PartyContactMethod> _contactMethods = [];
    private readonly List<PartyBankAccount> _bankAccounts = [];
    private readonly List<PartyRelationship> _relationships = [];

    // EF Core constructor
    private Party()
    {
        Name = null!;
    }

    public Party(PartyType partyType, string name)
    {
        Id = Guid.NewGuid();
        PartyType = partyType;
        Name = name;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void AddAddress(PartyAddress address) => _addresses.Add(address);
    public void AddContactMethod(PartyContactMethod contactMethod) => _contactMethods.Add(contactMethod);
    public void AddBankAccount(PartyBankAccount bankAccount) => _bankAccounts.Add(bankAccount);

    public void AddOrganizationDetails(string? vatNumber, string? chamberOfCommerceNumber)
    {
        if (PartyType != PartyType.Organization)
            throw new InvalidOperationException(
                "OrganizationDetails kan alleen toegevoegd worden aan een Party van type Organization.");
        OrganizationDetails = new OrganizationDetails(Id, vatNumber, chamberOfCommerceNumber);
    }

    public void RegisterAsCustomer(int debtorNumber, decimal discount, bool isVatShifted, short paymentTermDays,
        decimal? creditLimit)
    {
        CustomerRole = new CustomerRole(Id, debtorNumber, discount, isVatShifted, paymentTermDays, creditLimit);
    }

    public void RegisterAsSupplier(int supplierNumber, bool isVatShifted, short paymentTermDays)
    {
        SupplierRole = new SupplierRole(Id, supplierNumber, isVatShifted, paymentTermDays);
    }

    public void AddPersonDetails(string firstName, string? middleName, string lastName, string? initials = null)
    {
        if (PartyType != PartyType.Person)
            throw new InvalidOperationException(
                "PersonDetails kan alleen toegevoegd worden aan een Party van type Person.");
        PersonDetails = new PersonDetails(Id, firstName, middleName, lastName, initials);
    }

    public bool IsCustomer => CustomerRole is not null;
    public bool IsSupplier => SupplierRole is not null;

    // Reconstitute - alleen voor repository gebruik, niet voor nieuwe parties
    public static Party Reconstitute(
        Guid id, PartyType partyType, string name, bool isActive,
        DateTime createdAt, DateTime updatedAt,
        PersonDetails? personDetails, OrganizationDetails? organizationDetails,
        CustomerRole? customerRole, SupplierRole? supplierRole,
        IList<PartyAddress> addresses, IList<PartyContactMethod> contactMethods,
        IList<PartyBankAccount> bankAccounts)
    {
        var party = new Party
        {
            Id = id,
            PartyType = partyType,
            Name = name,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            PersonDetails = personDetails,
            OrganizationDetails = organizationDetails,
            CustomerRole = customerRole,
            SupplierRole = supplierRole
        };
        party._addresses.AddRange(addresses);
        party._contactMethods.AddRange(contactMethods);
        party._bankAccounts.AddRange(bankAccounts);
        return party;
    }

    public void UpdateOrganization(string name, string? vatNumber, string? chamberOfCommerceNumber)
    {
        if (PartyType != PartyType.Organization)
            throw new InvalidOperationException("UpdateOrganization kan alleen op een Party van type Organization.");
        Name = name;
        UpdatedAt = DateTime.UtcNow;
        OrganizationDetails?.Update(vatNumber, chamberOfCommerceNumber);
    }

    public void UpdatePerson(string firstName, string? middleName, string lastName, string? initials)
    {
        if (PartyType != PartyType.Person)
            throw new InvalidOperationException("UpdatePerson kan alleen op een Party van type Person.");
        PersonDetails?.Update(firstName, middleName, lastName, initials);
        Name = string.IsNullOrEmpty(middleName)
            ? $"{firstName} {lastName}"
            : $"{firstName} {middleName} {lastName}";
        UpdatedAt = DateTime.UtcNow;
    }
}