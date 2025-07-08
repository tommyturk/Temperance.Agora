using System.Text.Json;
using Temperance.Agora.Models;
using Temperance.Agora.Services.Interfaces;
using Temperance.Agora.Settings;

namespace Temperance.Agora.Services.Implementations
{
    public class StockService : IStockService
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly IAlpacaConfigSettings _alpacaConfigSettings;
        private readonly ILogger<StockService> _logger;
        public StockService(IHttpClientFactory httpClientFactory, IAlpacaConfigSettings alpacaConfigSettings, ILogger<StockService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _alpacaConfigSettings = alpacaConfigSettings;
            _logger = logger;
        }
        public async Task<AlpacaLatestQuoteResponse?> GetLatestStockQuoteAsync(string symbol)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("AlpacaSandboxClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca Data API.");
                return null;
            }

            var requestUri = $"/v2/stocks/{symbol.ToUpper()}/quotes/latest";

            _logger.LogDebug("Requesting latest quote for {Symbol} from Alpaca Data API: {Url}", symbol, httpClient.BaseAddress + requestUri.TrimStart('/'));

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("APCA-API-KEY-ID", _alpacaConfigSettings.PaperApiKeyId);
            request.Headers.Add("APCA-API-SECRET-KEY", _alpacaConfigSettings.PaperApiSecretKey);
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                _logger.LogInformation($"Failed to fetch latest quote for {symbol} from Alpaca. Status: {response.StatusCode}");

            string responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var alpacaQuote = JsonSerializer.Deserialize<AlpacaLatestQuoteResponse>(responseBody, options);
            return alpacaQuote;
        }
    }
}
