using IcMarkets.Infrastructure;
using IcMarkets.UseCases;
using IcMarkets.UseCases.FetchAllData;
using IcMarkets.UseCases.FetchData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IcMarkets.UnitTests;

public class FetchAllDataUnitTests
{
    private readonly DbContextOptions<IcMarketsDbContext> _dbContextOptions;

    public FetchAllDataUnitTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<IcMarketsDbContext>()
            .UseInMemoryDatabase(databaseName: $"IcMarketsTest_{nameof(FetchAllDataUnitTests)}")
            .Options;
    }

    [Fact]
    public async Task FetchAllData_All_Pairs_Successful()
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
                Content = new StringContent("{\n  \"name\": \"ETH.main\",\n  \"height\": 25712355,\n  \"hash\": \"532a95d00fcd012f8854206c180dd8769b73745e1443bd7357f0f4888c407dc9\",\n  \"time\": \"2026-08-08T19:32:28.526215776Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/532a95d00fcd012f8854206c180dd8769b73745e1443bd7357f0f4888c407dc9\",\n  \"previous_hash\": \"0063b069c52e0718130e812fa3db0090043f2555878c17926da7a42d81c93a18\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/0063b069c52e0718130e812fa3db0090043f2555878c17926da7a42d81c93a18\",\n  \"peer_count\": 0,\n  \"unconfirmed_count\": 8,\n  \"high_gas_price\": 3547635188,\n  \"medium_gas_price\": 2227649547,\n  \"low_gas_price\": 1349299104,\n  \"high_priority_fee\": 1145570078,\n  \"medium_priority_fee\": 406307334,\n  \"low_priority_fee\": 131974758,\n  \"base_fee\": 68042511,\n  \"last_fork_height\": 25705012,\n  \"last_fork_hash\": \"40cc72781736112bc5b5f4190a41161d11688b41deb40487753010ed2852a02a\"\n}")
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.DASH}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"DASH.main\",\n  \"height\": 2518581,\n  \"hash\": \"0000000000000011fb1118bff3cfccff2b4d34b2af5c54c04c2d970fca6a1048\",\n  \"time\": \"2026-08-08T19:30:39.195565853Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/dash/main/blocks/0000000000000011fb1118bff3cfccff2b4d34b2af5c54c04c2d970fca6a1048\",\n  \"previous_hash\": \"000000000000002917fad30b6ebd56e406593ed6402df5b7823a4801d2af8213\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/dash/main/blocks/000000000000002917fad30b6ebd56e406593ed6402df5b7823a4801d2af8213\",\n  \"peer_count\": 86,\n  \"unconfirmed_count\": 18,\n  \"high_fee_per_kb\": 24007,\n  \"medium_fee_per_kb\": 17613,\n  \"low_fee_per_kb\": 8572,\n  \"last_fork_height\": 2505218,\n  \"last_fork_hash\": \"000000000000000cd0b8c9f2e4678debe86a2120077c37f62b3366ee1c7577f6\"\n}")
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.BTC}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"BTC.main\",\n  \"height\": 961630,\n  \"hash\": \"000000000000000000000a7dda42d1e5936f9cd1686fe045bcb1c4fab5234c63\",\n  \"time\": \"2026-08-08T19:30:05.041736939Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/btc/main/blocks/000000000000000000000a7dda42d1e5936f9cd1686fe045bcb1c4fab5234c63\",\n  \"previous_hash\": \"00000000000000000000d67587a8c0259fce26d9dca808f4e93865d9b2dc7d1b\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/btc/main/blocks/00000000000000000000d67587a8c0259fce26d9dca808f4e93865d9b2dc7d1b\",\n  \"peer_count\": 324,\n  \"unconfirmed_count\": 6184,\n  \"high_fee_per_kb\": 3050,\n  \"medium_fee_per_kb\": 1786,\n  \"low_fee_per_kb\": 1391,\n  \"last_fork_height\": 949204,\n  \"last_fork_hash\": \"0000000000000000000003f026fbbf115f0f07d1b368dba6055c43e1eaddf76e\"\n}")
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.BTC}/{Constants.TEST3}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"BTC.test3\",\n  \"height\": 4997970,\n  \"hash\": \"00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e\",\n  \"time\": \"2026-08-07T21:01:46.532429651Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/btc/test3/blocks/00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e\",\n  \"previous_hash\": \"00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/btc/test3/blocks/00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f\",\n  \"peer_count\": 124,\n  \"unconfirmed_count\": 0,\n  \"high_fee_per_kb\": 11146,\n  \"medium_fee_per_kb\": 6347,\n  \"low_fee_per_kb\": 2707,\n  \"last_fork_height\": 4997086,\n  \"last_fork_hash\": \"0000000000eccda38eef88c5bbb88c53f953b650979166d43fd683db38ae8408\"\n}")
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.LTC}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"LTC.main\",\n  \"height\": 3156462,\n  \"hash\": \"71b3724fe551d09bf2f258732ac0da0569d00307b7ecd640a1cfbb3e070c9f17\",\n  \"time\": \"2026-08-08T19:30:47.276161496Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/ltc/main/blocks/71b3724fe551d09bf2f258732ac0da0569d00307b7ecd640a1cfbb3e070c9f17\",\n  \"previous_hash\": \"008d80465c0db1f7f0a315fc15217d62a01b666b08bfe9b85e8a7a137c7cd0c8\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/ltc/main/blocks/008d80465c0db1f7f0a315fc15217d62a01b666b08bfe9b85e8a7a137c7cd0c8\",\n  \"peer_count\": 324,\n  \"unconfirmed_count\": 782,\n  \"high_fee_per_kb\": 11808,\n  \"medium_fee_per_kb\": 9247,\n  \"low_fee_per_kb\": 8032,\n  \"last_fork_height\": 3154224,\n  \"last_fork_hash\": \"cb315cb1892af7619ebc0be65846e19ad261b6b478d416f5440046f9b9d40ddc\"\n}")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://any.ru")
        };

        var httpClientFactoryMoq = new Mock<IHttpClientFactory>();
        httpClientFactoryMoq.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var logger = new Mock<ILogger<FetchAllDataHandler>>();
        var handler = new FetchAllDataHandler(dbContext, httpClientFactoryMoq.Object, logger.Object);

        var startDate = DateTime.Now;

        var response = await handler.Handle(new FetchAllDataCommand(), default);

        var endDate = DateTime.Now;
        
        {
            var res = await dbContext.Blockchains.AllAsync(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);
            Assert.True(res);
        }

        {
            var expectedCount = Constants.CoinsToChainsMap.SelectMany(x => x.Value).Count();
            var actualCount = await dbContext.Blockchains.CountAsync(default);

            Assert.Equal(expectedCount, actualCount);
        }

        {
            var data = await dbContext.Blockchains.ToDictionaryAsync(x => x.Name, default);

            foreach (var item in Constants.CoinsToChainsMap)
            {
                var coin = item.Key;
                var chains = item.Value;

                foreach (var chain in chains)
                {
                    var name = $"{item.Key.ToUpper()}.{chain}";

                    Assert.True(data.ContainsKey(name));
                    Assert.NotNull(data[name].Hash);
                }
            }
        }
    }

    [Fact]
    public async Task FetchAllData_Some_Pairs_Successful_Some_Not()
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
                Content = new StringContent("{\n  \"name\": \"ETH.main\",\n  \"height\": 25712355,\n  \"hash\": \"532a95d00fcd012f8854206c180dd8769b73745e1443bd7357f0f4888c407dc9\",\n  \"time\": \"2026-08-08T19:32:28.526215776Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/532a95d00fcd012f8854206c180dd8769b73745e1443bd7357f0f4888c407dc9\",\n  \"previous_hash\": \"0063b069c52e0718130e812fa3db0090043f2555878c17926da7a42d81c93a18\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/eth/main/blocks/0063b069c52e0718130e812fa3db0090043f2555878c17926da7a42d81c93a18\",\n  \"peer_count\": 0,\n  \"unconfirmed_count\": 8,\n  \"high_gas_price\": 3547635188,\n  \"medium_gas_price\": 2227649547,\n  \"low_gas_price\": 1349299104,\n  \"high_priority_fee\": 1145570078,\n  \"medium_priority_fee\": 406307334,\n  \"low_priority_fee\": 131974758,\n  \"base_fee\": 68042511,\n  \"last_fork_height\": 25705012,\n  \"last_fork_hash\": \"40cc72781736112bc5b5f4190a41161d11688b41deb40487753010ed2852a02a\"\n}")
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.DASH}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"DASH.main\",\n  \"height\": 2518581,\n  \"hash\": \"0000000000000011fb1118bff3cfccff2b4d34b2af5c54c04c2d970fca6a1048\",\n  \"time\": \"2026-08-08T19:30:39.195565853Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/dash/main/blocks/0000000000000011fb1118bff3cfccff2b4d34b2af5c54c04c2d970fca6a1048\",\n  \"previous_hash\": \"000000000000002917fad30b6ebd56e406593ed6402df5b7823a4801d2af8213\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/dash/main/blocks/000000000000002917fad30b6ebd56e406593ed6402df5b7823a4801d2af8213\",\n  \"peer_count\": 86,\n  \"unconfirmed_count\": 18,\n  \"high_fee_per_kb\": 24007,\n  \"medium_fee_per_kb\": 17613,\n  \"low_fee_per_kb\": 8572,\n  \"last_fork_height\": 2505218,\n  \"last_fork_hash\": \"000000000000000cd0b8c9f2e4678debe86a2120077c37f62b3366ee1c7577f6\"\n}")
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.BTC}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.BTC}/{Constants.TEST3}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(request => request.RequestUri!.ToString().EndsWith($"{Constants.LTC}/{Constants.MAIN}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\n  \"name\": \"LTC.main\",\n  \"height\": 3156462,\n  \"hash\": \"71b3724fe551d09bf2f258732ac0da0569d00307b7ecd640a1cfbb3e070c9f17\",\n  \"time\": \"2026-08-08T19:30:47.276161496Z\",\n  \"latest_url\": \"https://api.blockcypher.com/v1/ltc/main/blocks/71b3724fe551d09bf2f258732ac0da0569d00307b7ecd640a1cfbb3e070c9f17\",\n  \"previous_hash\": \"008d80465c0db1f7f0a315fc15217d62a01b666b08bfe9b85e8a7a137c7cd0c8\",\n  \"previous_url\": \"https://api.blockcypher.com/v1/ltc/main/blocks/008d80465c0db1f7f0a315fc15217d62a01b666b08bfe9b85e8a7a137c7cd0c8\",\n  \"peer_count\": 324,\n  \"unconfirmed_count\": 782,\n  \"high_fee_per_kb\": 11808,\n  \"medium_fee_per_kb\": 9247,\n  \"low_fee_per_kb\": 8032,\n  \"last_fork_height\": 3154224,\n  \"last_fork_hash\": \"cb315cb1892af7619ebc0be65846e19ad261b6b478d416f5440046f9b9d40ddc\"\n}")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://any.ru")
        };

        var httpClientFactoryMoq = new Mock<IHttpClientFactory>();
        httpClientFactoryMoq.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var logger = new Mock<ILogger<FetchAllDataHandler>>();
        var handler = new FetchAllDataHandler(dbContext, httpClientFactoryMoq.Object, logger.Object);
        
        await handler.Handle(new FetchAllDataCommand(), default);

        var list = await dbContext.Blockchains.ToArrayAsync(default);

        foreach (var item in Constants.CoinsToChainsMap)
        {
            var coin = item.Key;
            var chains = item.Value;

            foreach (var chain in chains)
            {
                var name = $"{coin.ToUpper()}.{chain}";
                var blockchain = await dbContext.Blockchains.FirstOrDefaultAsync(x => x.Name == name, default);

                if (coin == Constants.BTC)
                {
                    Assert.Null(blockchain);
                }
                else
                {
                    Assert.NotNull(blockchain);
                    Assert.NotNull(blockchain.Hash);
                }
            }
        }
    }
}
