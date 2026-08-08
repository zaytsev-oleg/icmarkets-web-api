using IcMarkets.Infrastructure;
using IcMarkets.UseCases;
using IcMarkets.UseCases.FetchData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace IcMarkets.UnitTests;

public class FetchDataUnitTests
{
    private readonly DbContextOptions<IcMarketsDbContext> _dbContextOptions;

    public FetchDataUnitTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<IcMarketsDbContext>()
            .UseInMemoryDatabase(databaseName: $"IcMarketsTest_{nameof(FetchDataUnitTests)}")
            .Options;
    }

    [Fact]
    public async Task FetchData_ETH_Main_Success()
    {
        using var dbContext = new IcMarketsDbContext(_dbContextOptions);

        await dbContext.Database.EnsureDeletedAsync(default);
        await dbContext.Database.EnsureCreatedAsync(default);

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.ETH}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"ETH.main\",\n  \"height\": 25711382,\n  \"hash\": \"a1c54bc9d6279016d35c123f48e3b085d7c4214bd0049390872eed8c665219f7\",\n  \"time\": \"2026-08-08T16:17:03.69232353Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/a1c54bc9d6279016d35c123f48e3b085d7c4214bd0049390872eed8c665219f7\",\n  \"previous_hash\": \"00c5f90cfe9b81fd714121dc43e1990e4ff5b19d323467a70691f1911797938b\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/00c5f90cfe9b81fd714121dc43e1990e4ff5b19d323467a70691f1911797938b\",\n  \"peer_count\": 0,\n  \"unconfirmed_count\": 22,\n  \"high_gas_price\": 3547635188,\n  \"medium_gas_price\": 2209112079,\n  \"low_gas_price\": 1338087619,\n  \"high_priority_fee\": 1233547950,\n  \"medium_priority_fee\": 415546141,\n  \"low_priority_fee\": 133037838,\n  \"base_fee\": 69032367,\n  \"last_fork_height\": 25704142,\n  \"last_fork_hash\": \"073b33e38b4bffab6aa5717776a22be078980ffbce4dca13db3b40d9993063f2\"\n}")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://any.ru")
        };

        var httpClientFactoryMoq = new Mock<IHttpClientFactory>();
        httpClientFactoryMoq.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var logger = new Mock<ILogger<FetchDataHandler>>();

        var handler = new FetchDataHandler(dbContext, httpClientFactoryMoq.Object, logger.Object);
        var response = await handler.Handle(new FetchDataCommand(Constants.ETH, Constants.MAIN), default);

        {
            Assert.True(response.Pk > 0);
        }

        {
            var count = await dbContext.Blockchains.CountAsync(x => x.Pk == response.Pk, default);
            Assert.Equal(1, count);
        }

        {
            var count = await dbContext.Blockchains.CountAsync(x => x.Name == $"{Constants.ETH.ToUpper()}.{Constants.MAIN}", default);
            Assert.Equal(1, count);
        }
    }

    [Fact]
    public async Task FetchData_EmptyResponse()
    {
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://any.ru")
        };

        var httpClientFactoryMoq = new Mock<IHttpClientFactory>();
        httpClientFactoryMoq.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var logger = new Mock<ILogger<FetchDataHandler>>();

        var handler = new FetchDataHandler(null!, httpClientFactoryMoq.Object, logger.Object);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(new FetchDataCommand(Constants.ETH, Constants.MAIN), default));

        Assert.Contains(Constants.EmptyResponse, ex.Message);
    }

    [Fact]
    public async Task FetchData_Not_Found()
    {
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://any.ru")
        };

        var httpClientFactoryMoq = new Mock<IHttpClientFactory>();
        httpClientFactoryMoq.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var logger = new Mock<ILogger<FetchDataHandler>>();

        var handler = new FetchDataHandler(null!, httpClientFactoryMoq.Object, logger.Object);
        var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await handler.Handle(new FetchDataCommand(Constants.ETH, Constants.MAIN), default));
    }

    [Fact]
    public async Task FetchData_Wrond_Coin_To_Chain_Pair()
    {
        var handler = new FetchDataHandler(null!, null!, null!);
        var ex = await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(new FetchDataCommand(Constants.ETH, Constants.TEST3), default));
        Assert.Contains(Constants.NotSupportedCoinToChainPair, ex.Message);
    }
}
