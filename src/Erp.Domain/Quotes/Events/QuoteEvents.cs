using MediatR;

namespace Erp.Domain.Quotes.Events;

public record QuoteCreatedEvent(
    Guid      QuoteId,
    int       QuoteNumber,
    Guid?     CustomerId,
    string?   CustomerName,
    DateTime  OccurredAt) : INotification;

public record QuoteStatusChangedEvent(
    Guid      QuoteId,
    int       QuoteNumber,
    string    OldStatus,
    string    NewStatus,
    DateTime  OccurredAt) : INotification;

public record QuoteConvertedEvent(
    Guid                  QuoteId,
    int                   QuoteNumber,
    IReadOnlyList<Guid>   OrderIds,
    DateTime              OccurredAt) : INotification;
