using Dapper;
using Erp.Seeder.Models;
using Microsoft.Data.SqlClient;

namespace Erp.Seeder.Writers;

public class DatabaseWriter
{
    private readonly string _connectionString;

    public DatabaseWriter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        Console.WriteLine("  → Database leegmaken...");

        // Volgorde belangrijk vanwege foreign keys
        await conn.ExecuteAsync("DELETE FROM audit.party_history");
        await conn.ExecuteAsync("DELETE FROM audit.party_snapshots");
        await conn.ExecuteAsync("DELETE FROM audit.event_log");
        await conn.ExecuteAsync("DELETE FROM mdata.party_relationships");
        await conn.ExecuteAsync("DELETE FROM mdata.customer_roles");
        await conn.ExecuteAsync("DELETE FROM mdata.supplier_roles");
        await conn.ExecuteAsync("DELETE FROM mdata.party_addresses");
        await conn.ExecuteAsync("DELETE FROM mdata.party_contact_methods");
        await conn.ExecuteAsync("DELETE FROM mdata.party_bank_accounts");
        await conn.ExecuteAsync("DELETE FROM mdata.person_details");
        await conn.ExecuteAsync("DELETE FROM mdata.organization_details");
        await conn.ExecuteAsync("DELETE FROM mdata.parties");

        // Sequences resetten
        await conn.ExecuteAsync("ALTER SEQUENCE mdata.seq_debtor_number RESTART WITH 1000");
        await conn.ExecuteAsync("ALTER SEQUENCE mdata.seq_supplier_number RESTART WITH 1000");

        Console.WriteLine("  ✓ Database leeg");
    }

    public async Task WriteAsync(List<GeneratedParty> parties,
        List<PartyRelationshipRow> relationships,
        CancellationToken ct = default)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        Console.WriteLine($"  → {parties.Count} parties naar database...");

        // In batches van 500 voor performance
        const int batchSize = 500;
        var batches = parties.Chunk(batchSize).ToList();

        for (var i = 0; i < batches.Count; i++)
        {
            var batch = batches[i].ToList();
            await WriteBatchAsync(conn, batch, ct);
            Console.Write($"\r    Batch {i + 1}/{batches.Count} geschreven ({(i + 1) * batchSize} parties)");
            Console.WriteLine();
        }

        // Relaties
        if (relationships.Count > 0)
        {
            Console.WriteLine($"  → {relationships.Count} relaties naar database...");
            await conn.ExecuteAsync(@"
                INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
                VALUES (@FromPartyId, @ToPartyId, @RelationshipTypeId)",
                relationships);
        }

        Console.WriteLine("  ✓ Database klaar");
    }

    private static async Task WriteBatchAsync(SqlConnection conn,
        List<GeneratedParty> batch, CancellationToken ct)
    {
        using var tx = await conn.BeginTransactionAsync(ct);
        try
        {
            // Parties
            await conn.ExecuteAsync(@"
                INSERT INTO mdata.parties (id, party_type_id, name, is_active)
                VALUES (@Id, @PartyTypeId, @Name, @IsActive)",
                batch.Select(p => p.Party), tx);

            // Person details
            var persons = batch.Where(p => p.PersonDetail != null).ToList();
            if (persons.Count > 0)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.person_details
                        (party_id, first_name, middle_name, last_name, initials)
                    VALUES (@PartyId, @FirstName, @MiddleName, @LastName, @Initials)",
                    persons.Select(p => p.PersonDetail), tx);

            // Organization details
            var orgs = batch.Where(p => p.OrganizationDetail != null).ToList();
            if (orgs.Count > 0)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.organization_details
                        (party_id, vat_number, chamber_of_commerce_number)
                    VALUES (@PartyId, @VatNumber, @ChamberOfCommerceNumber)",
                    orgs.Select(p => p.OrganizationDetail), tx);

            // Adressen
            await conn.ExecuteAsync(@"
                INSERT INTO mdata.party_addresses
                    (party_id, address_type_id, street, house_number, house_number_addition,
                     postal_code, city, country_code, attention, is_default)
                VALUES (@PartyId, @AddressTypeId, @Street, @HouseNumber, @HouseNumberAddition,
                        @PostalCode, @City, @CountryCode, @Attention, @IsDefault)",
                batch.SelectMany(p => p.Addresses), tx);

            // Contactmethoden
            await conn.ExecuteAsync(@"
                INSERT INTO mdata.party_contact_methods
                    (party_id, contact_method_type_id, value, is_primary)
                VALUES (@PartyId, @ContactMethodTypeId, @Value, @IsPrimary)",
                batch.SelectMany(p => p.ContactMethods), tx);

            // Bankrekeningen
            var bankAccounts = batch.Where(p => p.BankAccount != null).ToList();
            if (bankAccounts.Count > 0)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.party_bank_accounts
                        (party_id, iban, bic, account_holder, is_primary)
                    VALUES (@PartyId, @Iban, @Bic, @AccountHolder, @IsPrimary)",
                    bankAccounts.Select(p => p.BankAccount), tx);

            // Customer roles
            var customers = batch.Where(p => p.CustomerRole != null).ToList();
            if (customers.Count > 0)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.customer_roles
                        (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
                    VALUES (@PartyId, @Discount, @IsVatShifted, @PaymentTermDays, @CreditLimit)",
                    customers.Select(p => p.CustomerRole), tx);

            // Supplier roles
            var suppliers = batch.Where(p => p.SupplierRole != null).ToList();
            if (suppliers.Count > 0)
                await conn.ExecuteAsync(@"
                    INSERT INTO mdata.supplier_roles
                        (party_id, is_vat_shifted, payment_term_days)
                    VALUES (@PartyId, @IsVatShifted, @PaymentTermDays)",
                    suppliers.Select(p => p.SupplierRole), tx);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
