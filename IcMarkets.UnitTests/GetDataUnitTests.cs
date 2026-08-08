using IcMarkets.Domain;
using IcMarkets.Infrastructure;
using IcMarkets.UseCases;
using IcMarkets.UseCases.GetData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace IcMarkets.UnitTests;

public class GetDataUnitTests
{
    private readonly DbContextOptions<IcMarketsDbContext> _dbContextOptions;

    public GetDataUnitTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<IcMarketsDbContext>()
            .UseInMemoryDatabase(databaseName: $"IcMarketsTest_{nameof(GetDataUnitTests)}")
            .Options;
    }

    [Fact]
    public async Task GetDataHandler_Unfiltered_And_Filtered_Queries()
    {
        using var dbContext = new IcMarketsDbContext(_dbContextOptions);

        await dbContext.Database.EnsureDeletedAsync(default);
        await dbContext.Database.EnsureCreatedAsync(default);

        var sampleData = GetSampleData();
        dbContext.AddRange(sampleData);
        await dbContext.SaveChangesAsync(default);

        var count = await dbContext.Blockchains.CountAsync(default);
        Assert.Equal(sampleData.Length, count);

        var logger = new Mock<ILogger<GetDataHandler>>();
        var handler = new GetDataHandler(dbContext, logger.Object);

        // Unfiltered query
        {
            var data = await handler.Handle(new GetDataQuery(null, null), default);
            Assert.Equal(sampleData.Length, data.Count);
        }

        // Filtered (by Name) queries
        {
            var data = await handler.Handle(new GetDataQuery(Constants.ETH.ToUpper(), Constants.MAIN), default);
            Assert.Equal(sampleData.Count(x => x.Name == $"{Constants.ETH.ToUpper()}.{Constants.MAIN}"), data.Count);
        }

        {
            var data = await handler.Handle(new GetDataQuery(Constants.LTC.ToUpper(), Constants.MAIN), default);
            Assert.Equal(sampleData.Count(x => x.Name == $"{Constants.LTC.ToUpper()}.{Constants.MAIN}"), data.Count);
        }

        {
            var data = await handler.Handle(new GetDataQuery(Constants.BTC.ToUpper(), Constants.MAIN), default);
            Assert.Equal(sampleData.Count(x => x.Name == $"{Constants.BTC.ToUpper()}.{Constants.MAIN}"), data.Count);
        }
    }

    [Fact]
    public async Task GetDataHandler_Sorting_Order()
    {
        using var dbContext = new IcMarketsDbContext(_dbContextOptions);

        await dbContext.Database.EnsureDeletedAsync(default);
        await dbContext.Database.EnsureCreatedAsync(default);

        var sampleData = GetSampleData();
        dbContext.AddRange(sampleData);
        await dbContext.SaveChangesAsync(default);

        var count = await dbContext.Blockchains.CountAsync(default);
        Assert.Equal(sampleData.Length, count);

        var logger = new Mock<ILogger<GetDataHandler>>();
        var handler = new GetDataHandler(dbContext, logger.Object);

        var data = await handler.Handle(new GetDataQuery(null, null), default);
        Assert.Equal(sampleData.OrderByDescending(x => x.CreatedAt), data.Data, (item1, item2) => item1.Hash == item2.Hash);

        data = await handler.Handle(new GetDataQuery(Constants.ETH.ToUpper(), Constants.MAIN), default);
        Assert.Equal(sampleData.Where(x => x.Name == $"{Constants.ETH.ToUpper()}.{Constants.MAIN}").OrderByDescending(x => x.CreatedAt),
            data.Data, (item1, item2) => item1.Hash == item2.Hash);

        data = await handler.Handle(new GetDataQuery(Constants.LTC.ToUpper(), Constants.MAIN), default);
        Assert.Equal(sampleData.Where(x => x.Name == $"{Constants.LTC.ToUpper()}.{Constants.MAIN}").OrderByDescending(x => x.CreatedAt),
            data.Data, (item1, item2) => item1.Hash == item2.Hash);
    }

    private static Blockchain[] GetSampleData()
    {
        Blockchain[] data = [
            new Blockchain
            {
                Name = "BTC.test3",
                Height = 4997970,
                Hash = "00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e",
                Time = DateTime.Parse("2026-08-08 00:31:37.226 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/btc/test3/blocks/00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e",
                PreviousHash = "00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f",
                PreviousUrl = "https://api.blockcypher.com/v1/btc/test3/blocks/00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f",
                PeerCount = 150,
                HighFeePerKb = 11146,
                MediumFeePerKb = 6347,
                LowFeePerKb = 2707,
                UnconfirmedCount = 0,
                LastForkHeight = 4997086,
                LastForkHash = "0000000000eccda38eef88c5bbb88c53f953b650979166d43fd683db38ae8408",
                CreatedAt = DateTime.Parse("2026-08-08 16:24:26.459 +0300"),
            },
            new Blockchain
            {
                Name = "DASH.main",
                Height = 2518437,
                Hash = "00000000000000032e8109a94a23613d497557fc96fdcbefd08959ee053102fc",
                Time = DateTime.Parse("2026-08-08 16:13:16.085 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/dash/main/blocks/00000000000000032e8109a94a23613d497557fc96fdcbefd08959ee053102fc",
                PreviousHash = "000000000000000392f64ffcca159b6f247505a9f2cdc05d960395902e958d5c",
                PreviousUrl = "https://api.blockcypher.com/v1/dash/main/blocks/000000000000000392f64ffcca159b6f247505a9f2cdc05d960395902e958d5c",
                PeerCount = 80,
                HighFeePerKb = 24007,
                MediumFeePerKb = 18541,
                LowFeePerKb = 11969,
                UnconfirmedCount = 34,
                LastForkHeight = 2505218,
                LastForkHash = "000000000000000cd0b8c9f2e4678debe86a2120077c37f62b3366ee1c7577f6",
                CreatedAt = DateTime.Parse("2026-08-08 16:24:26.451 +0300"),
            },
            new Blockchain
            {
                Name = "LTC.main",
                Height = 3156317,
                Hash = "869c1f715b30c8fc2268c11b093b58e06aed566e6db2114b78f380d8c7f02211",
                Time = DateTime.Parse("2026-08-08 16:22:24.089 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/ltc/main/blocks/869c1f715b30c8fc2268c11b093b58e06aed566e6db2114b78f380d8c7f02211",
                PreviousHash = "0034139adf0e8b012c245ab37495fb296a28d3d8ea6e84146f3a5bd4d3bee4da",
                PreviousUrl = "https://api.blockcypher.com/v1/ltc/main/blocks/0034139adf0e8b012c245ab37495fb296a28d3d8ea6e84146f3a5bd4d3bee4da",
                PeerCount = 323,
                HighFeePerKb = 11808,
                MediumFeePerKb = 9315,
                LowFeePerKb = 8040,
                UnconfirmedCount = 506,
                LastForkHeight = 3154224,
                LastForkHash = "cb315cb1892af7619ebc0be65846e19ad261b6b478d416f5440046f9b9d40ddc",
                CreatedAt = DateTime.Parse("2026-08-08 16:24:26.458 +0300"),
            },
            new Blockchain
            {
                Name = "ETH.main",
                Height = 25710520,
                Hash = "85810d51e8055b756a2ebecbaaedd43565b2a711d85ad675c50d091108b9585e",
                Time = DateTime.Parse("2026-08-08 16:24:16.672 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/eth/main/blocks/85810d51e8055b756a2ebecbaaedd43565b2a711d85ad675c50d091108b9585e",
                PreviousHash = "005ab046908ea0c08f8da80930c4b8624ef6d68585af6d9975015eecf11f2002",
                PreviousUrl = "https://api.blockcypher.com/v1/eth/main/blocks/005ab046908ea0c08f8da80930c4b8624ef6d68585af6d9975015eecf11f2002",
                PeerCount = 0,
                HighFeePerKb = 0,
                MediumFeePerKb = 0,
                LowFeePerKb = 0,
                UnconfirmedCount = 57,
                LastForkHeight = 25703908,
                LastForkHash = "cf22167b5c1fa06b1d2f3080d9c39a6071a9ed746d4e78e7fba6678f1977e5c1",
                CreatedAt = DateTime.Parse("2026-08-08 16:24:26.452 +0300"),
            },
            new Blockchain
            {
                Name = "BTC.main",
                Height = 961588,
                Hash = "00000000000000000000bcc6da6e4e8bcc0302a6c7ce236a7b61f585291dc334",
                Time = DateTime.Parse("2026-08-08 16:11:48.986 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/btc/main/blocks/00000000000000000000bcc6da6e4e8bcc0302a6c7ce236a7b61f585291dc334",
                PreviousHash = "0000000000000000000126e6867616a15e2d72fdb1cd63f972bbcee84562b07f",
                PreviousUrl = "https://api.blockcypher.com/v1/btc/main/blocks/0000000000000000000126e6867616a15e2d72fdb1cd63f972bbcee84562b07f",
                PeerCount = 324,
                HighFeePerKb = 2974,
                MediumFeePerKb = 1801,
                LowFeePerKb = 1399,
                UnconfirmedCount = 7162,
                LastForkHeight = 949204,
                LastForkHash = "0000000000000000000003f026fbbf115f0f07d1b368dba6055c43e1eaddf76e",
                CreatedAt = DateTime.Parse("2026-08-08 16:24:26.457 +0300"),
            },
            new Blockchain
            {
                Name = "ETH.main",
                Height = 25710643,
                Hash = "457d37e53a365d0be633478b1fd79813131d11ffcec87b928c4f90191a1182d7",
                Time = DateTime.Parse("2026-08-08 16:48:52.441 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/eth/main/blocks/457d37e53a365d0be633478b1fd79813131d11ffcec87b928c4f90191a1182d7",
                PreviousHash = "00a54be6ac3cc85d02bc153a844dee08ae90e949825e396d4ab69be2f1a0871e",
                PreviousUrl = "https://api.blockcypher.com/v1/eth/main/blocks/00a54be6ac3cc85d02bc153a844dee08ae90e949825e396d4ab69be2f1a0871e",
                PeerCount = 0,
                HighFeePerKb = 0,
                MediumFeePerKb = 0,
                LowFeePerKb = 0,
                UnconfirmedCount = 9,
                LastForkHeight = 25703908,
                LastForkHash = "cf22167b5c1fa06b1d2f3080d9c39a6071a9ed746d4e78e7fba6678f1977e5c1",
                CreatedAt = DateTime.Parse("2026-08-08 16:48:58.503 +0300"),
            },
            new Blockchain
            {
                Name = "LTC.main",
                Height = 3156323,
                Hash = "fe48699f474246f53139df101774eabe0225a7f7cf91eefc1fb4a8e22a63931e",
                Time = DateTime.Parse("2026-08-08 16:43:44.394 +0300"),
                LatestUrl = "https://api.blockcypher.com/v1/ltc/main/blocks/fe48699f474246f53139df101774eabe0225a7f7cf91eefc1fb4a8e22a63931e",
                PreviousHash = "0035752f61ebc0bd9cf238b4e95004da92b8de89135e386adf18a16e0ab8eac1",
                PreviousUrl = "https://api.blockcypher.com/v1/ltc/main/blocks/0035752f61ebc0bd9cf238b4e95004da92b8de89135e386adf18a16e0ab8eac1",
                PeerCount = 323,
                HighFeePerKb = 11808,
                MediumFeePerKb = 9281,
                LowFeePerKb = 8040,
                UnconfirmedCount = 969,
                LastForkHeight = 3154224,
                LastForkHash = "cb315cb1892af7619ebc0be65846e19ad261b6b478d416f5440046f9b9d40ddc",
                CreatedAt = DateTime.Parse("2026-08-08 16:49:22.033 +0300"),
            },
            /*
            new Blockchain
            {
                Pk = 0,
                Name = "",
                Height = 0,
                Hash = "",
                Time = DateTime.Parse(""),
                LatestUrl = "",
                PreviousHash = "",
                PreviousUrl = "",
                PeerCount = 0,
                HighFeePerKb = 0,
                MediumFeePerKb = 0,
                LowFeePerKb = 0,
                UnconfirmedCount = 0,
                LastForkHeight = 0,
                LastForkHash = "",
                CreatedAt = DateTime.Parse(""),
            },
            */
        ];

        return data;
    }
}