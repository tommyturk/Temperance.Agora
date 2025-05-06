using System.Text.Json.Serialization;

public class Position
{
    /// <summary>
    /// Asset ID (For options this represents the option contract ID).
    /// </summary>
    [JsonPropertyName("asset_id")]
    public Guid AssetId { get; set; }

    /// <summary>
    /// Symbol name of the asset.
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Represents the current exchanges Alpaca supports.
    /// </summary>
    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// This represents the category to which the asset belongs (e.g., "us_equity", "us_option", "crypto").
    /// </summary>
    [JsonPropertyName("asset_class")]
    public string AssetClass { get; set; } = string.Empty;

    /// <summary>
    /// Average entry price of the position. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("avg_entry_price")]
    public string AvgEntryPrice { get; set; } = string.Empty;

    /// <summary>
    /// The number of shares (or contracts for options). Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("qty")]
    public string Qty { get; set; } = string.Empty;

    /// <summary>
    /// Total number of shares available minus open orders / locked for options covered call. Stored as string.
    /// </summary>
    [JsonPropertyName("qty_available")]
    public string QtyAvailable { get; set; } = string.Empty;

    /// <summary>
    /// The side of the position ("long").
    /// </summary>
    [JsonPropertyName("side")]
    public string Side { get; set; } = string.Empty;

    /// <summary>
    /// Total dollar amount of the position. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("market_value")]
    public string MarketValue { get; set; } = string.Empty;

    /// <summary>
    /// Total cost basis in dollars. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("cost_basis")]
    public string CostBasis { get; set; } = string.Empty;

    /// <summary>
    /// Unrealized profit/loss in dollars. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("unrealized_pl")]
    public string UnrealizedPl { get; set; } = string.Empty;

    /// <summary>
    /// Unrealized profit/loss percent (by a factor of 1). Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("unrealized_plpc")]
    public string UnrealizedPlpc { get; set; } = string.Empty;

    /// <summary>
    /// Unrealized profit/loss in dollars for the day. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("unrealized_intraday_pl")]
    public string UnrealizedIntradayPl { get; set; } = string.Empty;

    /// <summary>
    /// Unrealized profit/loss percent (by a factor of 1) for the day. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("unrealized_intraday_plpc")]
    public string UnrealizedIntradayPlpc { get; set; } = string.Empty;

    /// <summary>
    /// Current asset price per share. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("current_price")]
    public string CurrentPrice { get; set; } = string.Empty;

    /// <summary>
    /// Last day’s asset price per share based on the closing value of the last trading day. Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("lastday_price")]
    public string LastdayPrice { get; set; } = string.Empty;

    /// <summary>
    /// Percent change from last day price (by a factor of 1). Stored as string to maintain precision.
    /// </summary>
    [JsonPropertyName("change_today")]
    public string ChangeToday { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the asset is marginable.
    /// </summary>
    [JsonPropertyName("asset_marginable")]
    public bool AssetMarginable { get; set; }
}

// To deserialize the response array, you would use:
// List<Position>? positions = JsonSerializer.Deserialize<List<Position>>(jsonString);
