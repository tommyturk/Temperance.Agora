using System.Text.Json.Serialization;

namespace TradingBot.Agora.Models
{
    public class OrderResponse
    {
        public List<Order> Orders { get; set; }
    }
}
