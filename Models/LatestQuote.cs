using System.Text.Json.Serialization;

namespace Temperance.Agora.Models
{
    public class LatestQuote
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime TimestampUtc { get; set; } // System.Text.Json can parse RFC3339 to DateTime

        [JsonPropertyName("ask_exchange")]
        public string? AskExchange { get; set; }

        [JsonPropertyName("ask_price")]
        public decimal AskPrice { get; set; }

        [JsonPropertyName("ask_size")]
        public long AskSize { get; set; } // Alpaca typically returns actual share count

        [JsonPropertyName("bid_exchange")]
        public string? BidExchange { get; set; }

        [JsonPropertyName("bid_price")]
        public decimal BidPrice { get; set; }

        [JsonPropertyName("bid_size")]
        public long BidSize { get; set; }

        [JsonPropertyName("conditions")]
        public List<string>? Conditions { get; set; }

        [JsonPropertyName("tape")]
        public string? Tape { get; set; }
    }
}
