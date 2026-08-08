using IcMarkets.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using IcMarkets.Domain;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IcMarkets.UseCases.FetchData;

public class FetchDataHandler : IRequestHandler<FetchDataCommand, FetchDataResponse>
{
    private IcMarketsDbContext _dbContext;
    private IHttpClientFactory _httpClientFactory;
    private ILogger<FetchDataHandler> _logger;

    public FetchDataHandler(IcMarketsDbContext dbContext, IHttpClientFactory httpClientFactory, ILogger<FetchDataHandler> logger)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<FetchDataResponse> Handle(FetchDataCommand request, CancellationToken ct)
    {
        var coinsToChainsMap = Constants.CoinsToChainsMap;

        if (!(coinsToChainsMap.ContainsKey(request.Coin) && coinsToChainsMap[request.Coin].Contains(request.Chain)))
        {
            throw new Exception(Constants.NotSupportedCoinToChainPair);
        }

        var httpClient = _httpClientFactory.CreateClient(Constants.BlockCypher);
        var response = await httpClient.GetAsync($"{request.Coin}/{request.Chain}", ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"{Constants.RequestFailed}: {response.RequestMessage?.RequestUri}, {response.StatusCode}({(int)response.StatusCode}).");
            response.EnsureSuccessStatusCode();
        }

        if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
        {
            var msg = $"{Constants.EmptyResponse}: {response.RequestMessage?.RequestUri}.";
            _logger.LogError(msg);
            throw new Exception(msg);
        }

        var obj = await response.Content.ReadFromJsonAsync<Blockchain>(ct);

        _dbContext.Blockchains.Add(obj!);
        await _dbContext.SaveChangesAsync(ct);

        return new FetchDataResponse(obj!.Pk);
    }
}
