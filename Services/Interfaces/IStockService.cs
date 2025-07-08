using Temperance.Agora.Models;

namespace Temperance.Agora.Services.Interfaces
{
    public interface IStockService
    {
        Task<AlpacaLatestQuoteResponse?> GetLatestStockQuoteAsync(string symbol);
    }
}
