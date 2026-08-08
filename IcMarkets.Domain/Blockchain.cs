using System.Text.Json.Serialization;

namespace IcMarkets.Domain;

/// <summary>
/// Blockchain model: https://www.blockcypher.com/dev/bitcoin/#blockchain.
/// </summary>
public class Blockchain
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Pk { get; set; }

    /// <summary>
    /// The name of the blockchain represented, in the form of $COIN.$CHAIN.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The current height of the blockchain; i.e., the number of blocks in the blockchain.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// The hash of the latest confirmed block in the blockchain; in Bitcoin, the hashing function is SHA256(SHA256(block)).
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// The time of the latest update to the blockchain; typically when the latest block was added.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    /// <summary>
    /// The BlockCypher URL to query for more information on the latest confirmed block; returns a Block.
    /// </summary>
    [JsonPropertyName("latest_url")]
    public string LatestUrl { get; set; } = string.Empty;

    /// <summary>
    /// The hash of the second-to-latest confirmed block in the blockchain.
    /// </summary>
    [JsonPropertyName("previous_hash")]
    public string PreviousHash { get; set; } = string.Empty;

    /// <summary>
    /// The BlockCypher URL to query for more information on the second-to-latest confirmed block; returns a Block.
    /// </summary>
    [JsonPropertyName("previous_url")]
    public string PreviousUrl { get; set; } = string.Empty;

    /// <summary>
    /// N/A, will be deprecated soon.
    /// </summary>
    [JsonPropertyName("peer_count")]
    public int? PeerCount { get; set; }

    /// <summary>
    /// A rolling average of the fee (in satoshis) paid per kilobyte for transactions to be confirmed within 1 to 2 blocks.
    /// </summary>
    [JsonPropertyName("high_fee_per_kb")]
    public int HighFeePerKb { get; set; }

    /// <summary>
    /// A rolling average of the fee (in satoshis) paid per kilobyte for transactions to be confirmed within 3 to 6 blocks.
    /// </summary>
    [JsonPropertyName("medium_fee_per_kb")]
    public int MediumFeePerKb { get; set; }

    /// <summary>
    /// A rolling average of the fee (in satoshis) paid per kilobyte for transactions to be confirmed in 7 or more blocks.
    /// </summary>
    [JsonPropertyName("low_fee_per_kb")]
    public int LowFeePerKb { get; set; }

    /// <summary>
    /// Number of unconfirmed transactions in memory pool (likely to be included in next block).
    /// </summary>
    [JsonPropertyName("unconfirmed_count")]
    public int UnconfirmedCount { get; set; }

    /// <summary>
    /// The current height of the latest fork to the blockchain; when no competing blockchain fork present, not returned with endpoints that return Blockchains.
    /// </summary>
    [JsonPropertyName("last_fork_height")]
    public int? LastForkHeight { get; set; }

    /// <summary>
    /// The hash of the latest confirmed block in the latest fork of the blockchain; when no competing blockchain fork present, not returned with endpoints that return Blockchains.
    /// </summary>
    [JsonPropertyName("last_fork_hash")]
    public string? LastForkHash { get; set; }

    /// <summary>
    /// Additional timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
