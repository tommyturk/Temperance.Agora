using System.Text.Json.Serialization;
public class Asset
{
    /// <summary>
    /// The unique identifier for the asset.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// The category to which the asset belongs (e.g., "us_equity", "us_option", "crypto").
    /// </summary>
    [JsonPropertyName("class")]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// The CUSIP identifier for the asset (US Equities only). Nullable.
    /// </summary>
    [JsonPropertyName("cusip")]
    public string? Cusip { get; set; }

    /// <summary>
    /// The exchange where the asset is traded.
    /// </summary>
    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    /// The symbol of the asset.
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// The official name of the asset.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The current status of the asset (e.g., "active", "inactive").
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the asset is tradable on Alpaca.
    /// </summary>
    [JsonPropertyName("tradable")]
    public bool Tradable { get; set; }

    /// <summary>
    /// Indicates if the asset is marginable.
    /// </summary>
    [JsonPropertyName("marginable")]
    public bool Marginable { get; set; }

    /// <summary>
    /// Indicates if the asset is shortable.
    /// </summary>
    [JsonPropertyName("shortable")]
    public bool Shortable { get; set; }

    /// <summary>
    /// Indicates if the asset is easy-to-borrow for shorting.
    /// </summary>
    [JsonPropertyName("easy_to_borrow")]
    public bool EasyToBorrow { get; set; }

    /// <summary>
    /// Indicates if the asset is fractionable.
    /// </summary>
    [JsonPropertyName("fractionable")]
    public bool Fractionable { get; set; }

    /// <summary>
    /// [DEPRECATED] Shows the margin requirement percentage for the asset (equities only). Use MarginRequirementLong or MarginRequirementShort instead.
    /// </summary>
    [JsonPropertyName("maintenance_margin_requirement")]
    [Obsolete("Please use MarginRequirementLong or MarginRequirementShort instead.")]
    public decimal? MaintenanceMarginRequirement { get; set; }

    /// <summary>
    /// The margin requirement percentage for the asset's long positions (equities only).
    /// Stored as a string as per API spec.
    /// </summary>
    [JsonPropertyName("margin_requirement_long")]
    public string MarginRequirementLong { get; set; } = string.Empty;

    /// <summary>
    /// The margin requirement percentage for the asset's short positions (equities only).
    /// Stored as a string as per API spec.
    /// </summary>
    [JsonPropertyName("margin_requirement_short")]
    public string MarginRequirementShort { get; set; } = string.Empty;

    /// <summary>
    /// A list of unique characteristics or attributes of the asset.
    /// </summary>
    [JsonPropertyName("attributes")]
    public List<string> Attributes { get; set; } = new List<string>();
}

// When deserializing the response array, you will deserialize into a List of Asset objects.
// For example:
// List<Asset> assetsResponse = JsonSerializer.Deserialize<List<Asset>>(jsonString);