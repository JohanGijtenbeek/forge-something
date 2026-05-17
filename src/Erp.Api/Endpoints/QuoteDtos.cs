using Erp.Domain.Quotes;

namespace Erp.Api.Endpoints;

// ============================================================
// RESPONSES
// ============================================================

public record QuoteSummaryResponse(
    Guid      Id,
    int       QuoteNumber,
    string?   CustomerName,
    DateOnly  Date,
    string    Status,
    int       LineCount,
    DateTime  CreatedAt
);

public record QuoteDetailResponse(
    Guid      Id,
    int       QuoteNumber,
    Guid?     CustomerId,
    string?   CustomerName,
    DateOnly  Date,
    string?   Reference,
    string?   ContactPerson,
    string?   DeliveryTime,
    decimal   HourlyRate,
    decimal   MaterialMargin,
    decimal   StandardMargin,
    decimal   SetupTime,
    string    Status,
    string?   Remarks,
    DateTime  CreatedAt,
    DateTime  UpdatedAt,
    IEnumerable<QuoteLineResponse> Lines
);

public record QuoteLineResponse(
    Guid     Id,
    int      SortOrder,
    string   PartName,
    string   PartNumber,
    decimal  Quantity,
    Guid?    ArticleId,
    string?  MaterialType,
    string?  MaterialCode,
    string?  MaterialCode2,
    string?  MaterialGeometry,
    decimal? MaterialSizeMm,
    decimal? MaterialLengthMm,
    decimal? MaterialQuantity,
    decimal? MaterialPrice,
    string   MaterialSource,
    int      OperationCount,
    decimal  OperationTimeMinutes,
    int      SubcontractingCount,
    decimal  SubcontractingPrice,
    decimal? TotalPricePerUnit,
    bool     IsManualPrice,
    decimal? ManualPrice,
    bool     IsAccepted,
    string?  Remarks
);

public record QuoteHistoryEntryResponse(
    long     Id,
    string   EventType,
    string   Summary,
    string   ChangedBy,
    DateTime ChangedAt
);

public record ConvertQuoteResponse(
    Guid             QuoteId,
    IEnumerable<Guid> CreatedOrderIds
);

// ============================================================
// REQUESTS
// ============================================================

public record CreateQuoteRequest(
    Guid?    CustomerId,
    DateOnly Date,
    string?  Reference,
    string?  ContactPerson,
    string?  DeliveryTime,
    decimal  HourlyRate,
    decimal  MaterialMargin,
    decimal  StandardMargin,
    decimal  SetupTime
);

public record UpdateQuoteHeaderRequest(
    Guid?    CustomerId,
    DateOnly Date,
    string?  Reference,
    string?  ContactPerson,
    string?  DeliveryTime,
    decimal  HourlyRate,
    decimal  MaterialMargin,
    decimal  StandardMargin,
    decimal  SetupTime,
    string?  Remarks
);

public record UpdateQuoteStatusRequest(string Status);

public record AddQuoteLineRequest(
    int      SortOrder,
    string   PartName,
    string   PartNumber,
    decimal  Quantity,
    Guid?    ArticleId,
    string?  MaterialType,
    string?  MaterialCode,
    string?  MaterialCode2,
    string?  MaterialGeometry,
    decimal? MaterialSizeMm,
    decimal? MaterialLengthMm,
    decimal? MaterialQuantity,
    decimal? MaterialPrice,
    string   MaterialSource,
    int      OperationCount,
    decimal  OperationTimeMinutes,
    int      SubcontractingCount,
    decimal  SubcontractingPrice,
    bool     IsManualPrice,
    decimal? ManualPrice,
    string?  Remarks
);

public record UpdateQuoteLineRequest(
    int      SortOrder,
    string   PartName,
    string   PartNumber,
    decimal  Quantity,
    Guid?    ArticleId,
    string?  MaterialType,
    string?  MaterialCode,
    string?  MaterialCode2,
    string?  MaterialGeometry,
    decimal? MaterialSizeMm,
    decimal? MaterialLengthMm,
    decimal? MaterialQuantity,
    decimal? MaterialPrice,
    string   MaterialSource,
    int      OperationCount,
    decimal  OperationTimeMinutes,
    int      SubcontractingCount,
    decimal  SubcontractingPrice,
    bool     IsManualPrice,
    decimal? ManualPrice,
    string?  Remarks
);

// ============================================================
// MAPPER
// ============================================================

public static class QuoteMapper
{
    public static QuoteLineResponse ToLineResponse(QuoteLine l) =>
        new(l.Id, l.SortOrder, l.PartName, l.PartNumber, l.Quantity, l.ArticleId,
            l.MaterialType, l.MaterialCode, l.MaterialCode2, l.MaterialGeometry,
            l.MaterialSizeMm, l.MaterialLengthMm, l.MaterialQuantity, l.MaterialPrice,
            l.MaterialSource, l.OperationCount, l.OperationTimeMinutes,
            l.SubcontractingCount, l.SubcontractingPrice,
            l.TotalPricePerUnit, l.IsManualPrice, l.ManualPrice, l.IsAccepted, l.Remarks);

    public static QuoteDetailResponse ToDetailResponse(Quote q, IEnumerable<QuoteLine> lines) =>
        new(q.Id, q.QuoteNumber, q.CustomerId, q.CustomerName, q.Date,
            q.Reference, q.ContactPerson, q.DeliveryTime,
            q.HourlyRate, q.MaterialMargin, q.StandardMargin, q.SetupTime,
            q.Status, q.Remarks, q.CreatedAt, q.UpdatedAt,
            lines.Select(ToLineResponse));
}
