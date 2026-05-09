namespace Erp.Domain.Parties.Events;

public record PartyCreatedEvent(
    Guid PartyId,
    string Name,
    PartyType PartyType,
    bool IsCustomer,
    bool IsSupplier,
    DateTime OccurredAt
);

public record PartyDeactivatedEvent(
    Guid PartyId,
    string Name,
    DateTime OccurredAt
);

public record PartyUpdatedEvent(
    Guid PartyId,
    string Name,
    DateTime OccurredAt
);
