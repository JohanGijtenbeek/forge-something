namespace Erp.Domain.Parties;

public record PartyHistoryEntry(
    long Id,
    string EventType,
    string Summary,
    string ChangedBy,
    DateTime ChangedAt
);
