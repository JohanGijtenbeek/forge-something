using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;

namespace Erp.Infrastructure.Health;

public class MeilisearchHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration config) : IHealthCheck
{
    private readonly string _url = config["Meilisearch:Url"] ?? "http://localhost:7700";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_url}/health", ct);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy($"Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}