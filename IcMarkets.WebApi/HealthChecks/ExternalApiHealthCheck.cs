using IcMarkets.Domain;
using IcMarkets.UseCases;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IcMarkets.WebApi.HealthChecks;

public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalApiHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.BlockCypher);

        var response = await httpClient.GetAsync($"{Constants.ETH}/{Constants.MAIN}", ct);

        if (!response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Unhealthy($"{Constants.RequestFailed}: {response.RequestMessage!.RequestUri}, {response.StatusCode}({(int)response.StatusCode}).");
        }

        var result = await response.Content.ReadFromJsonAsync<Blockchain>(ct);

        if (result == null)
        {
            return HealthCheckResult.Unhealthy($"{Constants.EmptyResponse}: {response.RequestMessage!.RequestUri}.");
        }

        var name = $"{Constants.ETH.ToUpper()}.{Constants.MAIN}";

        if (result.Name != name)
        {
            return HealthCheckResult.Unhealthy($"Received wrong blockchain Name: {result.Name}. Expected: {name}.");
        }

        return HealthCheckResult.Healthy();
    }
}
