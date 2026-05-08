using Dapper;
using Erp.Domain.Parties.Events;
using Erp.Infrastructure.Persistence;
using MediatR;
using System.Text.Json;

namespace Erp.Infrastructure.Handlers;

// Materialiseert party_history bij elk domain event
public class MaterializePartyHistoryOnCreatedHandler : INotificationHandler<PartyCreatedEvent>
{
    private readonly DbConnectionFactory _factory;

    public MaterializePartyHistoryOnCreatedHandler(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(PartyCreatedEvent notification, CancellationToken ct)
    {
        using var conn = _factory.Create();

        // Haal het event_log id op van het zojuist opgeslagen event
        var eventId = await conn.QuerySingleAsync<long>(@"
            SELECT TOP 1 id FROM audit.event_log
            WHERE aggregate_id = @PartyId AND event_type = 'PartyCreated'
            ORDER BY id DESC",
            new { notification.PartyId });

        var snapshot = JsonSerializer.Serialize(notification);

        await conn.ExecuteAsync(@"
            INSERT INTO audit.party_history
                (party_id, event_sequence, event_type, summary, changed_by, changed_at, snapshot)
            VALUES
                (@PartyId, @EventSequence, @EventType, @Summary, @ChangedBy, @ChangedAt, @Snapshot)",
            new
            {
                notification.PartyId,
                EventSequence = eventId,
                EventType = "PartyCreated",
                Summary = $"Party aangemaakt: {notification.Name}",
                ChangedBy = "system", // placeholder
                ChangedAt = notification.OccurredAt,
                Snapshot = snapshot
            });

        // Snapshot aanmaken bij aanmaken — altijd een goed startpunt
        await CreateSnapshotAsync(conn, notification.PartyId, eventId, "state_closed", snapshot);
    }

    private static async Task CreateSnapshotAsync(
        Microsoft.Data.SqlClient.SqlConnection conn,
        Guid partyId, long atEventId, string triggerReason, string snapshot)
    {
        await conn.ExecuteAsync(@"
            INSERT INTO audit.party_snapshots
                (party_id, at_event_id, snapshot, trigger_reason)
            VALUES
                (@PartyId, @AtEventId, @Snapshot, @TriggerReason)",
            new { PartyId = partyId, AtEventId = atEventId, Snapshot = snapshot, TriggerReason = triggerReason });
    }
}

public class MaterializePartyHistoryOnDeactivatedHandler : INotificationHandler<PartyDeactivatedEvent>
{
    private readonly DbConnectionFactory _factory;

    public MaterializePartyHistoryOnDeactivatedHandler(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(PartyDeactivatedEvent notification, CancellationToken ct)
    {
        using var conn = _factory.Create();

        var eventId = await conn.QuerySingleAsync<long>(@"
            SELECT TOP 1 id FROM audit.event_log
            WHERE aggregate_id = @PartyId AND event_type = 'PartyDeactivated'
            ORDER BY id DESC",
            new { notification.PartyId });

        var snapshot = JsonSerializer.Serialize(notification);

        await conn.ExecuteAsync(@"
            INSERT INTO audit.party_history
                (party_id, event_sequence, event_type, summary, changed_by, changed_at, snapshot)
            VALUES
                (@PartyId, @EventSequence, @EventType, @Summary, @ChangedBy, @ChangedAt, @Snapshot)",
            new
            {
                notification.PartyId,
                EventSequence = eventId,
                EventType = "PartyDeactivated",
                Summary = $"Party gedeactiveerd: {notification.Name}",
                ChangedBy = "system",
                ChangedAt = notification.OccurredAt,
                Snapshot = snapshot
            });

        // Deactiveren is een logisch afsluitsmoment — snapshot aanmaken
        await conn.ExecuteAsync(@"
            INSERT INTO audit.party_snapshots
                (party_id, at_event_id, snapshot, trigger_reason)
            VALUES
                (@PartyId, @AtEventId, @Snapshot, @TriggerReason)",
            new
            {
                PartyId = notification.PartyId,
                AtEventId = eventId,
                Snapshot = snapshot,
                TriggerReason = "state_closed"
            });
    }
}
