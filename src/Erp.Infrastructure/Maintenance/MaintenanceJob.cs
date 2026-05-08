using Erp.Infrastructure.Snapshots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Erp.Infrastructure.Maintenance;

// Draait als achtergrondproces — periodiek snapshots aanmaken en opschonen
public class MaintenanceJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MaintenanceJob> _logger;

    // Elke 24 uur
    private readonly TimeSpan _interval = TimeSpan.FromHours(24);

    public MaintenanceJob(IServiceScopeFactory scopeFactory, ILogger<MaintenanceJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Maintenance job gestart, interval: {Interval}", _interval);

        await Task.Delay(TimeSpan.FromSeconds(5), ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await RunAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout tijdens maintenance job");
            }

            await Task.Delay(_interval, ct);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        _logger.LogInformation("Maintenance job gestart om {Time}", DateTime.UtcNow);

        using var scope = _scopeFactory.CreateScope();
        var snapshotService = scope.ServiceProvider.GetRequiredService<SnapshotService>();

        var count = await snapshotService.CreateScheduledSnapshotsAsync(ct);

        _logger.LogInformation("Maintenance job klaar — {Count} snapshots aangemaakt", count);

        // TODO: cold/archive migratie hier toevoegen zodra business rules bepaald zijn
    }
}
