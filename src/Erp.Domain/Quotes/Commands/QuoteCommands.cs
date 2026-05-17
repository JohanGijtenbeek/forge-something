using MediatR;

namespace Erp.Domain.Quotes.Commands;

public record CreateQuoteCommand(
    Guid?    CustomerId,
    DateOnly Date,
    string?  Reference,
    string?  ContactPerson,
    string?  DeliveryTime,
    decimal  HourlyRate,
    decimal  MaterialMargin,
    decimal  StandardMargin,
    decimal  SetupTime) : IRequest<Guid>;

public record UpdateQuoteHeaderCommand(
    Guid     QuoteId,
    Guid?    CustomerId,
    DateOnly Date,
    string?  Reference,
    string?  ContactPerson,
    string?  DeliveryTime,
    decimal  HourlyRate,
    decimal  MaterialMargin,
    decimal  StandardMargin,
    decimal  SetupTime,
    string?  Remarks) : IRequest;

public record UpdateQuoteStatusCommand(
    Guid   QuoteId,
    string NewStatus) : IRequest;

public record AddQuoteLineCommand(
    Guid     QuoteId,
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
    string?  Remarks) : IRequest<Guid>;

public record UpdateQuoteLineCommand(
    Guid     LineId,
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
    string?  Remarks) : IRequest;

public record RemoveQuoteLineCommand(Guid LineId) : IRequest;

public record AcceptQuoteLineCommand(Guid LineId) : IRequest;

public record ConvertQuoteToOrdersCommand(Guid QuoteId) : IRequest<IReadOnlyList<Guid>>;
