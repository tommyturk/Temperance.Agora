using Alpaca.Markets;
using TradingBot.Agora.Models;

namespace TradingBot.Agora.Services.Interfaces
{
    public interface IAlpacaTradingService
    {
        //Account
        Task<IAccount?> GetAccountAsync();

        //Orders & Positions
        Task<Order?> GetOrderById(Guid id);
        Task<List<Order?>> GetOrdersAsync();
        Task<Order?> PlaceOrderAsync(OrderPlacementRequest orderRequest);
        Task<List<Position?>> GetPositionsAsync();
        Task<Order> ClosePosition(string symbol, decimal? quantity, decimal? percentage);
        Task<List<Order?>> ClosePositions(bool cancelOrders);
        Task<Position?> GetOpenPositionAsync(string symbol);

        //Assets    
        Task<AssetsResponse> GetAssetsAsync();
    }
}
