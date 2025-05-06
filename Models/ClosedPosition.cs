namespace TradingBot.Agora.Models
{
    public class ClosedPosition
    {
        public string Symbol { get; set; }

        public string Status { get; set; }

        public OrderResponse Order {get;set;}
    }
}
