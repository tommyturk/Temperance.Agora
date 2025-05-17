using System.Text.Json.Serialization;

namespace Temperance.Agora.Models
{
    public class Order
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("client_order_id")]
        public string? ClientOrderId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("submitted_at")]
        public DateTimeOffset SubmittedAt { get; set; }

        // Add other relevant fields like symbol, qty, side, type, status, filled_qty, filled_avg_price etc.
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("qty")]
        public string Quantity { get; set; } = string.Empty; // Alpaca often returns strings

        [JsonPropertyName("filled_qty")]
        public string FilledQuantity { get; set; } = string.Empty;

        [JsonPropertyName("side")]
        public string Side { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("time_in_force")]
        public string TimeInForce { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("filled_avg_price")]
        public string? FilledAvgPrice { get; set; } // Nullable if not filled
    }
}
