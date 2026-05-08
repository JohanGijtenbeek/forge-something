using Dapper;
using Erp.Domain.Parties;
using Erp.Infrastructure.Persistence;

namespace Erp.Infrastructure.Repositories;

public class PartyRepository : IPartyRepository
{
    private readonly DbConnectionFactory _factory;

    public PartyRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<PartyRow>(
            "SELECT * FROM mdata.parties WHERE id = @Id",
            new { Id = id });
        return row?.ToDomain(null, null, null, null, [], [], []);
    }

    public async Task<Party?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        using var multi = await conn.QueryMultipleAsync(@"
            SELECT * FROM mdata.parties               WHERE id = @Id;
            SELECT * FROM mdata.person_details        WHERE party_id = @Id;
            SELECT * FROM mdata.organization_details  WHERE party_id = @Id;
            SELECT * FROM mdata.customer_roles        WHERE party_id = @Id;
            SELECT * FROM mdata.supplier_roles        WHERE party_id = @Id;
            SELECT * FROM mdata.party_addresses       WHERE party_id = @Id;
            SELECT * FROM mdata.party_contact_methods WHERE party_id = @Id;
            SELECT * FROM mdata.party_bank_accounts   WHERE party_id = @Id;",
            new { Id = id });

        var party          = await multi.ReadSingleOrDefaultAsync<PartyRow>();
        var personDetails  = await multi.ReadSingleOrDefaultAsync<PersonDetailsRow>();
        var orgDetails     = await multi.ReadSingleOrDefaultAsync<OrganizationDetailsRow>();
        var customerRole   = await multi.ReadSingleOrDefaultAsync<CustomerRoleRow>();
        var supplierRole   = await multi.ReadSingleOrDefaultAsync<SupplierRoleRow>();
        var addresses      = (await multi.ReadAsync<PartyAddressRow>()).ToList();
        var contactMethods = (await multi.ReadAsync<PartyContactMethodRow>()).ToList();
        var bankAccounts   = (await multi.ReadAsync<PartyBankAccountRow>()).ToList();

        return party?.ToDomain(personDetails, orgDetails, customerRole, supplierRole, addresses, contactMethods, bankAccounts);
    }

    public async Task<IReadOnlyList<Party>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<PartyRow>(@"
            SELECT * FROM mdata.parties
            WHERE @IncludeInactive = 1 OR is_active = 1
            ORDER BY name",
            new { IncludeInactive = includeInactive ? 1 : 0 });

        return rows.Select(r => r.ToDomain(null, null, null, null, [], [], [])).ToList();
    }

    public async Task<(IReadOnlyList<Party> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, bool includeInactive = false, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        using var multi = await conn.QueryMultipleAsync(@"
            SELECT COUNT(*) FROM mdata.parties
            WHERE @IncludeInactive = 1 OR is_active = 1;

            SELECT * FROM mdata.parties
            WHERE @IncludeInactive = 1 OR is_active = 1
            ORDER BY name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;",
            new { IncludeInactive = includeInactive ? 1 : 0, Offset = (page - 1) * pageSize, PageSize = pageSize });

        var totalCount = await multi.ReadSingleAsync<int>();
        var rows = await multi.ReadAsync<PartyRow>();
        return (rows.Select(r => r.ToDomain(null, null, null, null, [], [], [])).ToList(), totalCount);
    }

    public async Task<IReadOnlyList<Party>> GetCustomersAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<PartyRow>(@"
            SELECT p.* FROM mdata.parties p
            JOIN mdata.customer_roles cr ON cr.party_id = p.id
            WHERE @IncludeInactive = 1 OR p.is_active = 1
            ORDER BY p.name",
            new { IncludeInactive = includeInactive ? 1 : 0 });

        return rows.Select(r => r.ToDomain(null, null, null, null, [], [], [])).ToList();
    }

    public async Task<IReadOnlyList<Party>> GetSuppliersAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<PartyRow>(@"
            SELECT p.* FROM mdata.parties p
            JOIN mdata.supplier_roles sr ON sr.party_id = p.id
            WHERE @IncludeInactive = 1 OR p.is_active = 1
            ORDER BY p.name",
            new { IncludeInactive = includeInactive ? 1 : 0 });

        return rows.Select(r => r.ToDomain(null, null, null, null, [], [], [])).ToList();
    }

    public async Task AddAsync(Party party, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            await conn.ExecuteAsync(@"
                INSERT INTO mdata.parties (id, party_type_id, name, is_active, created_at, updated_at)
                VALUES (@Id, @PartyTypeId, @Name, @IsActive, @CreatedAt, @UpdatedAt)",
                new { party.Id, PartyTypeId = (int)party.PartyType, party.Name, party.IsActive, party.CreatedAt, party.UpdatedAt },
                tx);

            if (party.PersonDetails is not null)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials, created_at, updated_at)
                    VALUES (@PartyId, @FirstName, @MiddleName, @LastName, @Initials, @CreatedAt, @UpdatedAt)",
                    new { PartyId = party.Id, party.PersonDetails.FirstName, party.PersonDetails.MiddleName,
                          party.PersonDetails.LastName, party.PersonDetails.Initials,
                          party.PersonDetails.CreatedAt, party.PersonDetails.UpdatedAt },
                    tx);

            if (party.OrganizationDetails is not null)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number, created_at, updated_at)
                    VALUES (@PartyId, @VatNumber, @ChamberOfCommerceNumber, @CreatedAt, @UpdatedAt)",
                    new { PartyId = party.Id, party.OrganizationDetails.VatNumber,
                          party.OrganizationDetails.ChamberOfCommerceNumber,
                          party.OrganizationDetails.CreatedAt, party.OrganizationDetails.UpdatedAt },
                    tx);

            if (party.CustomerRole is not null)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit, created_at, updated_at)
                    VALUES (@PartyId, @Discount, @IsVatShifted, @PaymentTermDays, @CreditLimit, @CreatedAt, @UpdatedAt)",
                    new { PartyId = party.Id, party.CustomerRole.Discount, party.CustomerRole.IsVatShifted,
                          party.CustomerRole.PaymentTermDays, party.CustomerRole.CreditLimit,
                          party.CustomerRole.CreatedAt, party.CustomerRole.UpdatedAt },
                    tx);

            if (party.SupplierRole is not null)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days, created_at, updated_at)
                    VALUES (@PartyId, @IsVatShifted, @PaymentTermDays, @CreatedAt, @UpdatedAt)",
                    new { PartyId = party.Id, party.SupplierRole.IsVatShifted,
                          party.SupplierRole.PaymentTermDays,
                          party.SupplierRole.CreatedAt, party.SupplierRole.UpdatedAt },
                    tx);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE mdata.parties SET is_active = 0, updated_at = SYSUTCDATETIME() WHERE id = @Id",
            new { Id = id });
    }


    public async Task UpdateAsync(Party party, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            await conn.ExecuteAsync(@"
                UPDATE mdata.parties
                SET name = @Name, updated_at = @UpdatedAt
                WHERE id = @Id",
                new { party.Id, party.Name, party.UpdatedAt }, tx);

            if (party.OrganizationDetails is not null)
                await conn.ExecuteAsync(@"
                    UPDATE mdata.organization_details
                    SET vat_number = @VatNumber,
                        chamber_of_commerce_number = @ChamberOfCommerceNumber,
                        updated_at = @UpdatedAt
                    WHERE party_id = @PartyId",
                    new
                    {
                        PartyId = party.Id,
                        party.OrganizationDetails.VatNumber,
                        party.OrganizationDetails.ChamberOfCommerceNumber,
                        party.OrganizationDetails.UpdatedAt
                    }, tx);

            if (party.PersonDetails is not null)
                await conn.ExecuteAsync(@"
                    UPDATE mdata.person_details
                    SET first_name = @FirstName,
                        middle_name = @MiddleName,
                        last_name = @LastName,
                        initials = @Initials,
                        updated_at = @UpdatedAt
                    WHERE party_id = @PartyId",
                    new
                    {
                        PartyId = party.Id,
                        party.PersonDetails.FirstName,
                        party.PersonDetails.MiddleName,
                        party.PersonDetails.LastName,
                        party.PersonDetails.Initials,
                        party.PersonDetails.UpdatedAt
                    }, tx);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }


    public async Task<IReadOnlyList<PartyHistoryEntry>> GetHistoryAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<PartyHistoryEntry>(@"
            SELECT id, event_type AS EventType, summary AS Summary,
                   changed_by AS ChangedBy, changed_at AS ChangedAt
            FROM audit.party_history
            WHERE party_id = @Id
            ORDER BY changed_at DESC",
            new { Id = id });
        return rows.ToList();
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) { }
}

// ============================================================
// Dapper row models - platte mapping van database naar C#
// ============================================================

file record PartyRow
{
    public Guid Id { get; init; }
    public int PartyTypeId { get; init; }
    public string Name { get; init; } = "";
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public Party ToDomain(PersonDetailsRow? pd, OrganizationDetailsRow? od, CustomerRoleRow? cr, SupplierRoleRow? sr,
        IList<PartyAddressRow> addresses, IList<PartyContactMethodRow> contactMethods, IList<PartyBankAccountRow> bankAccounts)
        => Party.Reconstitute(Id, (PartyType)PartyTypeId, Name, IsActive, CreatedAt, UpdatedAt,
            pd?.ToDomain(), od?.ToDomain(), cr?.ToDomain(), sr?.ToDomain(),
            addresses.Select(a => a.ToDomain()).ToList(),
            contactMethods.Select(c => c.ToDomain()).ToList(),
            bankAccounts.Select(b => b.ToDomain()).ToList());
}

file record PersonDetailsRow
{
    public Guid PartyId { get; init; }
    public string FirstName { get; init; } = "";
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = "";
    public string? Initials { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public PersonDetails ToDomain()
        => PersonDetails.Reconstitute(PartyId, FirstName, MiddleName, LastName, Initials, CreatedAt, UpdatedAt);
}

file record OrganizationDetailsRow
{
    public Guid PartyId { get; init; }
    public string? VatNumber { get; init; }
    public string? ChamberOfCommerceNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public OrganizationDetails ToDomain()
        => OrganizationDetails.Reconstitute(PartyId, VatNumber, ChamberOfCommerceNumber, CreatedAt, UpdatedAt);
}

file record CustomerRoleRow
{
    public Guid PartyId { get; init; }
    public int DebtorNumber { get; init; }
    public decimal Discount { get; init; }
    public bool IsVatShifted { get; init; }
    public short PaymentTermDays { get; init; }
    public decimal? CreditLimit { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public CustomerRole ToDomain()
        => CustomerRole.Reconstitute(PartyId, DebtorNumber, Discount, IsVatShifted, PaymentTermDays, CreditLimit, CreatedAt, UpdatedAt);
}

file record SupplierRoleRow
{
    public Guid PartyId { get; init; }
    public int SupplierNumber { get; init; }
    public bool IsVatShifted { get; init; }
    public short PaymentTermDays { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public SupplierRole ToDomain()
        => SupplierRole.Reconstitute(PartyId, SupplierNumber, IsVatShifted, PaymentTermDays, CreatedAt, UpdatedAt);
}

file record PartyAddressRow
{
    public Guid Id { get; init; }
    public Guid PartyId { get; init; }
    public int AddressTypeId { get; init; }
    public string Street { get; init; } = "";
    public string HouseNumber { get; init; } = "";
    public string? HouseNumberAddition { get; init; }
    public string PostalCode { get; init; } = "";
    public string City { get; init; } = "";
    public string CountryCode { get; init; } = "NL";
    public string? Attention { get; init; }
    public bool IsDefault { get; init; }
    public DateTime CreatedAt { get; init; }

    public PartyAddress ToDomain()
        => PartyAddress.Reconstitute(Id, PartyId, (AddressType)AddressTypeId, Street, HouseNumber,
            HouseNumberAddition, PostalCode, City, CountryCode, Attention, IsDefault, CreatedAt);
}

file record PartyContactMethodRow
{
    public Guid Id { get; init; }
    public Guid PartyId { get; init; }
    public int ContactMethodTypeId { get; init; }
    public string Value { get; init; } = "";
    public bool IsPrimary { get; init; }
    public DateTime CreatedAt { get; init; }

    public PartyContactMethod ToDomain()
        => PartyContactMethod.Reconstitute(Id, PartyId, (ContactMethodType)ContactMethodTypeId, Value, IsPrimary, CreatedAt);
}

file record PartyBankAccountRow
{
    public Guid Id { get; init; }
    public Guid PartyId { get; init; }
    public string Iban { get; init; } = "";
    public string? Bic { get; init; }
    public string? AccountHolder { get; init; }
    public bool IsPrimary { get; init; }
    public DateTime CreatedAt { get; init; }

    public PartyBankAccount ToDomain()
        => PartyBankAccount.Reconstitute(Id, PartyId, Iban, Bic, AccountHolder, IsPrimary, CreatedAt);
}

// Partial class to add UpdateAsync
