using Dapper;
using Erp.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Erp.Infrastructure.Snapshots;

public class SnapshotService
{
    private readonly DbConnectionFactory _factory;
    private readonly ILogger<SnapshotService> _logger;

    // Na hoeveel events zonder snapshot wordt er automatisch een aangemaakt
    private const int EventCountThreshold = 50;

    public SnapshotService(DbConnectionFactory factory, ILogger<SnapshotService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    // Aangeroepen na elk event — checkt of event count threshold bereikt is
    public async Task CheckAndSnapshotIfNeededAsync(Guid partyId, long latestEventId)
    {
        using var conn = _factory.Create();

        // Hoeveel events zijn er sinds het laatste snapshot?
        var lastSnapshotEventId = await conn.QuerySingleOrDefaultAsync<long?>(@"
            SELECT TOP 1 at_event_id FROM audit.party_snapshots
            WHERE party_id = @PartyId
            ORDER BY at_event_id DESC",
            new { PartyId = partyId }) ?? 0;

        var eventsSinceSnapshot = await conn.QuerySingleAsync<int>(@"
            SELECT COUNT(*) FROM audit.event_log
            WHERE aggregate_id = @PartyId
            AND id > @LastSnapshotEventId",
            new { PartyId = partyId, LastSnapshotEventId = lastSnapshotEventId });

        if (eventsSinceSnapshot >= EventCountThreshold)
        {
            _logger.LogInformation(
                "Party {PartyId} heeft {Count} events sinds laatste snapshot — snapshot aanmaken",
                partyId, eventsSinceSnapshot);

            await CreateSnapshotAsync(conn, partyId, latestEventId, "event_count");
        }
    }

    // Aangeroepen door de maintenance job — scheduled trigger
    public async Task<int> CreateScheduledSnapshotsAsync(CancellationToken ct = default)
    {
        using var conn = _factory.Create();

        // Parties die events hebben maar geen recent snapshot
        var candidates = await conn.QueryAsync<Guid>(@"
            SELECT DISTINCT e.aggregate_id
            FROM audit.event_log e
            WHERE e.aggregate_type = 'Party'
            AND NOT EXISTS (
                SELECT 1 FROM audit.party_snapshots s
                WHERE s.party_id = e.aggregate_id
                AND s.created_at > DATEADD(DAY, -1, SYSUTCDATETIME())
            )",
            new { });

        var count = 0;
        foreach (var partyId in candidates)
        {
            if (ct.IsCancellationRequested) break;

            var latestEventId = await conn.QuerySingleAsync<long>(@"
                SELECT TOP 1 id FROM audit.event_log
                WHERE aggregate_id = @PartyId
                ORDER BY id DESC",
                new { PartyId = partyId });

            await CreateSnapshotAsync(conn, partyId, latestEventId, "scheduled");
            count++;
        }

        _logger.LogInformation("Scheduled snapshots aangemaakt: {Count}", count);
        return count;
    }

    private static async Task CreateSnapshotAsync(
        Microsoft.Data.SqlClient.SqlConnection conn,
        Guid partyId, long atEventId, string triggerReason)
    {
        // Haal de meest recente history entry op als snapshot basis
        var snapshot = await conn.QuerySingleOrDefaultAsync<string>(@"
            SELECT TOP 1 snapshot FROM audit.party_history
            WHERE party_id = @PartyId
            ORDER BY id DESC",
            new { PartyId = partyId });

        if (snapshot is null) return;

        await conn.ExecuteAsync(@"
            INSERT INTO audit.party_snapshots
                (party_id, at_event_id, snapshot, trigger_reason)
            VALUES
                (@PartyId, @AtEventId, @Snapshot, @TriggerReason)",
            new { PartyId = partyId, AtEventId = atEventId, Snapshot = snapshot, TriggerReason = triggerReason });
    }
}
