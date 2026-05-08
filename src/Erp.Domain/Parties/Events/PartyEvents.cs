using MediatR;

namespace Erp.Domain.Parties.Events;

// Domain events beschrijven wat er is gebeurd — verleden tijd
// INotification = MediatR event (geen return waarde, meerdere handlers mogelijk)

public record PartyCreatedEvent(
    Guid PartyId,
    string Name,
    PartyType PartyType,
    bool IsCustomer,
    bool IsSupplier,
    DateTime OccurredAt
) : INotification;

public record PartyDeactivatedEvent(
    Guid PartyId,
    string Name,
    DateTime OccurredAt
) : INotification;

public record PartyUpdatedEvent(
    Guid PartyId,
    string Name,
    DateTime OccurredAt
) : INotification;
