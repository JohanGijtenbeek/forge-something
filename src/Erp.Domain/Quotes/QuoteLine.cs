namespace Erp.Domain.Quotes;

public class QuoteLine
{
    public Guid     Id                    { get; private set; }
    public Guid     QuoteId               { get; private set; }
    public int      SortOrder             { get; private set; }
    public string   PartName              { get; private set; } = null!;
    public string   PartNumber            { get; private set; } = null!;
    public decimal  Quantity              { get; private set; }
    public Guid?    ArticleId             { get; private set; }

    // Material (denormalized — material catalog deferred)
    public string?  MaterialType          { get; private set; }
    public string?  MaterialCode          { get; private set; }
    public string?  MaterialCode2         { get; private set; }
    public string?  MaterialGeometry      { get; private set; }
    public decimal? MaterialSizeMm        { get; private set; }
    public decimal? MaterialLengthMm      { get; private set; }
    public decimal? MaterialQuantity      { get; private set; }
    public decimal? MaterialPrice         { get; private set; }
    public string   MaterialSource        { get; private set; } = "inclusive";

    // Operations
    public int      OperationCount        { get; private set; }
    public decimal  OperationTimeMinutes  { get; private set; }

    // Subcontracting
    public int      SubcontractingCount   { get; private set; }
    public decimal  SubcontractingPrice   { get; private set; }

    // Pricing
    public decimal? TotalPricePerUnit     { get; private set; }
    public bool     IsManualPrice         { get; private set; }
    public decimal? ManualPrice           { get; private set; }
    public bool     IsAccepted            { get; private set; }
    public string?  Remarks               { get; private set; }

    private QuoteLine() { }

    public static QuoteLine Create(
        Guid quoteId, int sortOrder, string partName, string partNumber, decimal quantity,
        Guid? articleId, string? materialType, string? materialCode, string? materialCode2,
        string? materialGeometry, decimal? materialSizeMm, decimal? materialLengthMm,
        decimal? materialQuantity, decimal? materialPrice, string materialSource,
        int operationCount, decimal operationTimeMinutes,
        int subcontractingCount, decimal subcontractingPrice,
        bool isManualPrice, decimal? manualPrice, string? remarks)
    {
        var line = new QuoteLine
        {
            Id                   = Guid.NewGuid(),
            QuoteId              = quoteId,
            SortOrder            = sortOrder,
            PartName             = partName,
            PartNumber           = partNumber,
            Quantity             = quantity,
            ArticleId            = articleId,
            MaterialType         = materialType,
            MaterialCode         = materialCode,
            MaterialCode2        = materialCode2,
            MaterialGeometry     = materialGeometry,
            MaterialSizeMm       = materialSizeMm,
            MaterialLengthMm     = materialLengthMm,
            MaterialQuantity     = materialQuantity,
            MaterialPrice        = materialPrice,
            MaterialSource       = materialSource,
            OperationCount       = operationCount,
            OperationTimeMinutes = operationTimeMinutes,
            SubcontractingCount  = subcontractingCount,
            SubcontractingPrice  = subcontractingPrice,
            IsManualPrice        = isManualPrice,
            ManualPrice          = manualPrice,
            Remarks              = remarks
        };
        line.TotalPricePerUnit = isManualPrice && manualPrice.HasValue
            ? manualPrice
            : null; // calculated by caller after creation
        return line;
    }

    public void Update(
        int sortOrder, string partName, string partNumber, decimal quantity,
        Guid? articleId, string? materialType, string? materialCode, string? materialCode2,
        string? materialGeometry, decimal? materialSizeMm, decimal? materialLengthMm,
        decimal? materialQuantity, decimal? materialPrice, string materialSource,
        int operationCount, decimal operationTimeMinutes,
        int subcontractingCount, decimal subcontractingPrice,
        bool isManualPrice, decimal? manualPrice, string? remarks)
    {
        SortOrder            = sortOrder;
        PartName             = partName;
        PartNumber           = partNumber;
        Quantity             = quantity;
        ArticleId            = articleId;
        MaterialType         = materialType;
        MaterialCode         = materialCode;
        MaterialCode2        = materialCode2;
        MaterialGeometry     = materialGeometry;
        MaterialSizeMm       = materialSizeMm;
        MaterialLengthMm     = materialLengthMm;
        MaterialQuantity     = materialQuantity;
        MaterialPrice        = materialPrice;
        MaterialSource       = materialSource;
        OperationCount       = operationCount;
        OperationTimeMinutes = operationTimeMinutes;
        SubcontractingCount  = subcontractingCount;
        SubcontractingPrice  = subcontractingPrice;
        IsManualPrice        = isManualPrice;
        ManualPrice          = manualPrice;
        Remarks              = remarks;
        TotalPricePerUnit    = isManualPrice && manualPrice.HasValue ? manualPrice : null;
    }

    public void SetCalculatedPrice(decimal price) => TotalPricePerUnit = price;

    public void Accept() => IsAccepted = true;

    // TODO: verify formula against legacy system
    public decimal CalculateTotalPricePerUnit(decimal hourlyRate, decimal materialMargin, decimal standardMargin, decimal setupTimeHours)
    {
        var laborCost    = (setupTimeHours + OperationCount * OperationTimeMinutes / 60m) * hourlyRate;
        var materialCost = (MaterialPrice ?? 0) * (MaterialQuantity ?? 0);
        return laborCost * (1 + standardMargin / 100m)
             + materialCost * (materialMargin / 100m)
             + SubcontractingPrice;
    }

    public static QuoteLine Reconstitute(
        Guid id, Guid quoteId, int sortOrder, string partName, string partNumber, decimal quantity,
        Guid? articleId, string? materialType, string? materialCode, string? materialCode2,
        string? materialGeometry, decimal? materialSizeMm, decimal? materialLengthMm,
        decimal? materialQuantity, decimal? materialPrice, string materialSource,
        int operationCount, decimal operationTimeMinutes,
        int subcontractingCount, decimal subcontractingPrice,
        decimal? totalPricePerUnit, bool isManualPrice, decimal? manualPrice,
        bool isAccepted, string? remarks) =>
        new()
        {
            Id                   = id,
            QuoteId              = quoteId,
            SortOrder            = sortOrder,
            PartName             = partName,
            PartNumber           = partNumber,
            Quantity             = quantity,
            ArticleId            = articleId,
            MaterialType         = materialType,
            MaterialCode         = materialCode,
            MaterialCode2        = materialCode2,
            MaterialGeometry     = materialGeometry,
            MaterialSizeMm       = materialSizeMm,
            MaterialLengthMm     = materialLengthMm,
            MaterialQuantity     = materialQuantity,
            MaterialPrice        = materialPrice,
            MaterialSource       = materialSource,
            OperationCount       = operationCount,
            OperationTimeMinutes = operationTimeMinutes,
            SubcontractingCount  = subcontractingCount,
            SubcontractingPrice  = subcontractingPrice,
            TotalPricePerUnit    = totalPricePerUnit,
            IsManualPrice        = isManualPrice,
            ManualPrice          = manualPrice,
            IsAccepted           = isAccepted,
            Remarks              = remarks
        };
}
