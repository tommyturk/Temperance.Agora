using System.Text.Json.Serialization;

namespace Temperance.Agora.Models
{
    public class OrderResponse
    {
        public List<Order> Orders { get; set; }
    }
}
