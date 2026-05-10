using Dapper;
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
}
