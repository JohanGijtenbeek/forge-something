using Dapper;
using Erp.Seeder.Generators;
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
        // audit (child tables first, event_log last)
        await conn.ExecuteAsync("DELETE FROM audit.quote_snapshots");
        await conn.ExecuteAsync("DELETE FROM audit.quote_history");
        await conn.ExecuteAsync("DELETE FROM audit.order_snapshots");
        await conn.ExecuteAsync("DELETE FROM audit.order_history");
        await conn.ExecuteAsync("DELETE FROM audit.article_history");
        await conn.ExecuteAsync("DELETE FROM audit.article_snapshots");
        await conn.ExecuteAsync("DELETE FROM audit.party_history");
        await conn.ExecuteAsync("DELETE FROM audit.party_snapshots");
        await conn.ExecuteAsync("DELETE FROM audit.event_log");
        // orders (child tables before production_orders, production_orders before articles/parties/quotes)
        await conn.ExecuteAsync("DELETE FROM mdata.order_bom_lines");
        await conn.ExecuteAsync("DELETE FROM mdata.order_operations");
        await conn.ExecuteAsync("DELETE FROM mdata.production_orders");
        // quotes (lines before quotes)
        await conn.ExecuteAsync("DELETE FROM mdata.quote_lines");
        await conn.ExecuteAsync("DELETE FROM mdata.quotes");
        // articles (operations and bom before articles, categories/uom last)
        await conn.ExecuteAsync("DELETE FROM mdata.article_operations");
        await conn.ExecuteAsync("DELETE FROM mdata.bill_of_materials");
        await conn.ExecuteAsync("DELETE FROM mdata.articles");
        await conn.ExecuteAsync("DELETE FROM mdata.article_categories");
        await conn.ExecuteAsync("DELETE FROM mdata.units_of_measure");
        // parties
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
        await conn.ExecuteAsync("ALTER SEQUENCE mdata.seq_article_number RESTART WITH 1000");
        await conn.ExecuteAsync("ALTER SEQUENCE mdata.seq_order_number RESTART WITH 1000");
        await conn.ExecuteAsync("ALTER SEQUENCE mdata.seq_quote_number RESTART WITH 1000");

        Console.WriteLine("  ✓ Database leeg");
    }

    public async Task WriteArticleReferenceDataAsync(CancellationToken ct = default)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        Console.WriteLine("  → Artikel referentiedata naar database...");

        foreach (var cat in ArticleGenerator.Categories)
        {
            await conn.ExecuteAsync(@"
                IF NOT EXISTS (SELECT 1 FROM mdata.article_categories WHERE id = @Id OR name = @Name)
                    INSERT INTO mdata.article_categories (id, name, sort_order)
                    VALUES (@Id, @Name, @SortOrder)",
                cat);
        }

        foreach (var uom in ArticleGenerator.UnitsOfMeasure)
        {
            await conn.ExecuteAsync(@"
                IF NOT EXISTS (SELECT 1 FROM mdata.units_of_measure WHERE id = @Id OR name = @Name)
                    INSERT INTO mdata.units_of_measure (id, name, abbreviation)
                    VALUES (@Id, @Name, @Abbreviation)",
                uom);
        }

        Console.WriteLine("  ✓ Referentiedata klaar");
    }
}
