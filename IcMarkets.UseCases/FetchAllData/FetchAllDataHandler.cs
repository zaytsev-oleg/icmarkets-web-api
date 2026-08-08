using IcMarkets.Domain;
using IcMarkets.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;

namespace IcMarkets.UseCases.FetchAllData;

public class FetchAllDataHandler : IRequestHandler<FetchAllDataCommand, FetchAllDataResponse>
{
    private IcMarketsDbContext _dbContext;
    private IHttpClientFactory _httpClientFactory;
    private ILogger<FetchAllDataHandler> _logger;

    public FetchAllDataHandler(IcMarketsDbContext dbContext, IHttpClientFactory httpClientFactory, ILogger<FetchAllDataHandler> logger)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<FetchAllDataResponse> Handle(FetchAllDataCommand command, CancellationToken ct)
    {
        var coinsToChainsMap = Constants.CoinsToChainsMap;

        var bag = new ConcurrentBag<Blockchain>();

        await Parallel.ForEachAsync(coinsToChainsMap, async (item, ct) =>
        {
            var coin = item.Key;
            var chains = item.Value;

            var httpClient = _httpClientFactory.CreateClient(Constants.BlockCypher);

            foreach (var chain in chains)
            {
                var response = await httpClient.GetAsync($"{coin}/{chain}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"{Constants.RequestFailed}: {response.RequestMessage?.RequestUri}, {response.StatusCode}({(int)response.StatusCode}).");
                    continue;
                }

                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    _logger.LogError($"{Constants.EmptyResponse}: {response.RequestMessage?.RequestUri}.");
                    continue;
                }

                var obj = await response.Content.ReadFromJsonAsync<Blockchain>(ct);
                bag.Add(obj!);
            }
        });

        if (!bag.IsEmpty)
        {
            _dbContext.AddRange(bag);
            await _dbContext.SaveChangesAsync(ct);
        }

        return new FetchAllDataResponse();
    }
}
