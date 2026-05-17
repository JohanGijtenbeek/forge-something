using Dapper;
using Erp.Domain.Quotes;
using Erp.Infrastructure.Persistence;

namespace Erp.Infrastructure.Repositories;

public class QuoteRepository : IQuoteRepository
{
    private readonly DbConnectionFactory _factory;

    public QuoteRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    private const string QuoteSelect = @"
        SELECT id, quote_number, customer_id, customer_name, date, reference, contact_person,
               delivery_time, hourly_rate, material_margin, standard_margin, setup_time,
               status, remarks, created_at, updated_at
        FROM mdata.quotes";

    public async Task<Quote?> GetByIdAsync(Guid id)
    {
        using var conn = _factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<QuoteRow>(
            $"{QuoteSelect} WHERE id = @Id", new { Id = id });
        return row?.ToDomain();
    }

    public async Task<IEnumerable<QuoteLine>> GetLinesAsync(Guid quoteId)
    {
        using var conn = _factory.Create();
        var rows = await conn.QueryAsync<QuoteLineRow>(@"
            SELECT id, quote_id, sort_order, part_name, part_number, quantity, article_id,
                   material_type, material_code, material_code2, material_geometry,
                   material_size_mm, material_length_mm, material_quantity, material_price,
                   material_source, operation_count, operation_time_minutes,
                   subcontracting_count, subcontracting_price,
                   total_price_per_unit, is_manual_price, manual_price, is_accepted, remarks
            FROM mdata.quote_lines
            WHERE quote_id = @QuoteId
            ORDER BY sort_order, id",
            new { QuoteId = quoteId });
        return rows.Select(r => r.ToDomain());
    }

    public async Task<QuoteLine?> GetLineAsync(Guid lineId)
    {
        using var conn = _factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<QuoteLineRow>(@"
            SELECT id, quote_id, sort_order, part_name, part_number, quantity, article_id,
                   material_type, material_code, material_code2, material_geometry,
                   material_size_mm, material_length_mm, material_quantity, material_price,
                   material_source, operation_count, operation_time_minutes,
                   subcontracting_count, subcontracting_price,
                   total_price_per_unit, is_manual_price, manual_price, is_accepted, remarks
            FROM mdata.quote_lines WHERE id = @Id",
            new { Id = lineId });
        return row?.ToDomain();
    }

    public async Task<(IEnumerable<Quote> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? status)
    {
        using var conn = _factory.Create();

        var where = new System.Text.StringBuilder("WHERE 1=1");
        if (!string.IsNullOrWhiteSpace(status))
            where.Append(" AND status = @Status");
        if (!string.IsNullOrWhiteSpace(search))
            where.Append(" AND (CAST(quote_number AS NVARCHAR) LIKE @Search OR customer_name LIKE @Search OR reference LIKE @Search)");

        var sql = $@"
            SELECT COUNT(*) FROM mdata.quotes {where};

            SELECT id, quote_number, customer_id, customer_name, date, reference, contact_person,
                   delivery_time, hourly_rate, material_margin, standard_margin, setup_time,
                   status, remarks, created_at, updated_at
            FROM mdata.quotes
            {where}
            ORDER BY created_at DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        using var multi = await conn.QueryMultipleAsync(sql, new
        {
            Status = status,
            Search = $"%{search}%",
            Offset = (page - 1) * pageSize,
            PageSize = pageSize
        });

        var total = await multi.ReadSingleAsync<int>();
        var rows  = await multi.ReadAsync<QuoteRow>();
        return (rows.Select(r => r.ToDomain()), total);
    }

    public async Task<IEnumerable<QuoteHistoryEntry>> GetHistoryAsync(Guid quoteId)
    {
        using var conn = _factory.Create();
        return await conn.QueryAsync<QuoteHistoryEntry>(@"
            SELECT id, event_type AS EventType, summary AS Summary,
                   changed_by AS ChangedBy, changed_at AS ChangedAt
            FROM audit.quote_history
            WHERE quote_id = @Id
            ORDER BY changed_at DESC",
            new { Id = quoteId });
    }

    public async Task SaveAsync(Quote quote)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO mdata.quotes
                (id, quote_number, customer_id, customer_name, date, reference, contact_person,
                 delivery_time, hourly_rate, material_margin, standard_margin, setup_time,
                 status, remarks, created_at, updated_at)
            VALUES
                (@Id, @QuoteNumber, @CustomerId, @CustomerName, @Date, @Reference, @ContactPerson,
                 @DeliveryTime, @HourlyRate, @MaterialMargin, @StandardMargin, @SetupTime,
                 @Status, @Remarks, @CreatedAt, @UpdatedAt)",
            new
            {
                quote.Id, quote.QuoteNumber, quote.CustomerId, quote.CustomerName, quote.Date,
                quote.Reference, quote.ContactPerson, quote.DeliveryTime,
                quote.HourlyRate, quote.MaterialMargin, quote.StandardMargin, quote.SetupTime,
                quote.Status, quote.Remarks, quote.CreatedAt, quote.UpdatedAt
            });
    }

    public async Task UpdateHeaderAsync(Quote quote)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            UPDATE mdata.quotes SET
                customer_id = @CustomerId, customer_name = @CustomerName, date = @Date,
                reference = @Reference, contact_person = @ContactPerson,
                delivery_time = @DeliveryTime, hourly_rate = @HourlyRate,
                material_margin = @MaterialMargin, standard_margin = @StandardMargin,
                setup_time = @SetupTime, remarks = @Remarks, updated_at = @UpdatedAt
            WHERE id = @Id",
            new
            {
                quote.CustomerId, quote.CustomerName, quote.Date, quote.Reference,
                quote.ContactPerson, quote.DeliveryTime, quote.HourlyRate,
                quote.MaterialMargin, quote.StandardMargin, quote.SetupTime,
                quote.Remarks, quote.UpdatedAt, quote.Id
            });
    }

    public async Task UpdateStatusAsync(Quote quote)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE mdata.quotes SET status = @Status, updated_at = @UpdatedAt WHERE id = @Id",
            new { quote.Status, quote.UpdatedAt, quote.Id });
    }

    public async Task AddLineAsync(QuoteLine line)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            INSERT INTO mdata.quote_lines
                (id, quote_id, sort_order, part_name, part_number, quantity, article_id,
                 material_type, material_code, material_code2, material_geometry,
                 material_size_mm, material_length_mm, material_quantity, material_price,
                 material_source, operation_count, operation_time_minutes,
                 subcontracting_count, subcontracting_price,
                 total_price_per_unit, is_manual_price, manual_price, is_accepted, remarks)
            VALUES
                (@Id, @QuoteId, @SortOrder, @PartName, @PartNumber, @Quantity, @ArticleId,
                 @MaterialType, @MaterialCode, @MaterialCode2, @MaterialGeometry,
                 @MaterialSizeMm, @MaterialLengthMm, @MaterialQuantity, @MaterialPrice,
                 @MaterialSource, @OperationCount, @OperationTimeMinutes,
                 @SubcontractingCount, @SubcontractingPrice,
                 @TotalPricePerUnit, @IsManualPrice, @ManualPrice, @IsAccepted, @Remarks)",
            new
            {
                line.Id, line.QuoteId, line.SortOrder, line.PartName, line.PartNumber,
                line.Quantity, line.ArticleId, line.MaterialType, line.MaterialCode,
                line.MaterialCode2, line.MaterialGeometry, line.MaterialSizeMm,
                line.MaterialLengthMm, line.MaterialQuantity, line.MaterialPrice,
                line.MaterialSource, line.OperationCount, line.OperationTimeMinutes,
                line.SubcontractingCount, line.SubcontractingPrice,
                line.TotalPricePerUnit, line.IsManualPrice, line.ManualPrice,
                line.IsAccepted, line.Remarks
            });
    }

    public async Task UpdateLineAsync(QuoteLine line)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(@"
            UPDATE mdata.quote_lines SET
                sort_order = @SortOrder, part_name = @PartName, part_number = @PartNumber,
                quantity = @Quantity, article_id = @ArticleId,
                material_type = @MaterialType, material_code = @MaterialCode,
                material_code2 = @MaterialCode2, material_geometry = @MaterialGeometry,
                material_size_mm = @MaterialSizeMm, material_length_mm = @MaterialLengthMm,
                material_quantity = @MaterialQuantity, material_price = @MaterialPrice,
                material_source = @MaterialSource, operation_count = @OperationCount,
                operation_time_minutes = @OperationTimeMinutes,
                subcontracting_count = @SubcontractingCount,
                subcontracting_price = @SubcontractingPrice,
                total_price_per_unit = @TotalPricePerUnit, is_manual_price = @IsManualPrice,
                manual_price = @ManualPrice, is_accepted = @IsAccepted, remarks = @Remarks
            WHERE id = @Id",
            new
            {
                line.SortOrder, line.PartName, line.PartNumber, line.Quantity, line.ArticleId,
                line.MaterialType, line.MaterialCode, line.MaterialCode2, line.MaterialGeometry,
                line.MaterialSizeMm, line.MaterialLengthMm, line.MaterialQuantity,
                line.MaterialPrice, line.MaterialSource, line.OperationCount,
                line.OperationTimeMinutes, line.SubcontractingCount, line.SubcontractingPrice,
                line.TotalPricePerUnit, line.IsManualPrice, line.ManualPrice,
                line.IsAccepted, line.Remarks, line.Id
            });
    }

    public async Task RemoveLineAsync(Guid lineId)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync("DELETE FROM mdata.quote_lines WHERE id = @Id", new { Id = lineId });
    }
}

// ============================================================
// Dapper row models
// ============================================================

file record QuoteRow
{
    public Guid     Id             { get; init; }
    public int      QuoteNumber    { get; init; }
    public Guid?    CustomerId     { get; init; }
    public string?  CustomerName   { get; init; }
    public DateOnly Date           { get; init; }
    public string?  Reference      { get; init; }
    public string?  ContactPerson  { get; init; }
    public string?  DeliveryTime   { get; init; }
    public decimal  HourlyRate     { get; init; }
    public decimal  MaterialMargin { get; init; }
    public decimal  StandardMargin { get; init; }
    public decimal  SetupTime      { get; init; }
    public string   Status         { get; init; } = "";
    public string?  Remarks        { get; init; }
    public DateTime CreatedAt      { get; init; }
    public DateTime UpdatedAt      { get; init; }

    public Quote ToDomain() => Quote.Reconstitute(
        Id, QuoteNumber, CustomerId, CustomerName, Date, Reference, ContactPerson,
        DeliveryTime, HourlyRate, MaterialMargin, StandardMargin, SetupTime,
        Status, Remarks, CreatedAt, UpdatedAt);
}

file record QuoteLineRow
{
    public Guid     Id                    { get; init; }
    public Guid     QuoteId               { get; init; }
    public int      SortOrder             { get; init; }
    public string   PartName              { get; init; } = "";
    public string   PartNumber            { get; init; } = "";
    public decimal  Quantity              { get; init; }
    public Guid?    ArticleId             { get; init; }
    public string?  MaterialType          { get; init; }
    public string?  MaterialCode          { get; init; }
    public string?  MaterialCode2         { get; init; }
    public string?  MaterialGeometry      { get; init; }
    public decimal? MaterialSizeMm        { get; init; }
    public decimal? MaterialLengthMm      { get; init; }
    public decimal? MaterialQuantity      { get; init; }
    public decimal? MaterialPrice         { get; init; }
    public string   MaterialSource        { get; init; } = "inclusive";
    public int      OperationCount        { get; init; }
    public decimal  OperationTimeMinutes  { get; init; }
    public int      SubcontractingCount   { get; init; }
    public decimal  SubcontractingPrice   { get; init; }
    public decimal? TotalPricePerUnit     { get; init; }
    public bool     IsManualPrice         { get; init; }
    public decimal? ManualPrice           { get; init; }
    public bool     IsAccepted            { get; init; }
    public string?  Remarks               { get; init; }

    public QuoteLine ToDomain() => QuoteLine.Reconstitute(
        Id, QuoteId, SortOrder, PartName, PartNumber, Quantity, ArticleId,
        MaterialType, MaterialCode, MaterialCode2, MaterialGeometry,
        MaterialSizeMm, MaterialLengthMm, MaterialQuantity, MaterialPrice, MaterialSource,
        OperationCount, OperationTimeMinutes, SubcontractingCount, SubcontractingPrice,
        TotalPricePerUnit, IsManualPrice, ManualPrice, IsAccepted, Remarks);
}
