using System.Collections.Frozen;

namespace IcMarkets.UseCases;

public class Constants
{
    public const string BlockCypher = "BlockCypher";

    // Coins
    public const string ETH  = "eth";
    public const string DASH = "dash";
    public const string BTC  = "btc";
    public const string LTC  = "ltc";

    // Chains
    public const string MAIN  = "main";
    public const string TEST3 = "test3";

    // Supported pairs
    public static FrozenDictionary<string, string[]> CoinsToChainsMap =>
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { ETH,  [MAIN] },
            { DASH, [MAIN] },
            { BTC,  [MAIN, TEST3] },
            { LTC,  [MAIN] },
        }.ToFrozenDictionary();

    // Errors
    public const string NotSupportedCoinToChainPair = "Not supported coin to chain pair";
    public const string EmptyResponse = "Empty response";
    public const string RequestFailed = "Request failed";
}
