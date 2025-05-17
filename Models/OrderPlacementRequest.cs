using Alpaca.Markets;

namespace Temperance.Agora.Models
{
    public class OrderPlacementRequest
    {
        public required string Symbol { get; set; }
        public decimal Quantity { get; set; }
        public OrderSide Side { get; set; }
        public OrderType Type { get; set; }
        public required TimeInForce TimeInForce { get; set; }
        public decimal? LimitPrice { get; set; } 
        public string? ClientOrderId { get; set; }
    }
}
