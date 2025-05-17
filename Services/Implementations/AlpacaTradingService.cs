using Alpaca.Markets;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Temperance.Agora.Models;
using Temperance.Agora.Services.Interfaces;
using Temperance.Agora.Settings;

namespace Temperance.Agora.Services.Implementations
{
    public class AlpacaTradingService : IAlpacaTradingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AlpacaConfigSettings _alpacaConfigSettings;
        private readonly ILogger<AlpacaTradingService> _logger;

        public AlpacaTradingService(IHttpClientFactory httpClientFactory,
            IOptions<AlpacaConfigSettings> alpacaConfigSettings, ILogger<AlpacaTradingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _alpacaConfigSettings = alpacaConfigSettings.Value;
            _logger = logger;
        }

        public async Task<IAccount?> GetAccountAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }

            var requestUri = "/v2/account";
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            //request.Headers.Add("APCA-API-KEY-ID", _alpacaConfigSettings.PaperApiKeyId);
            //request.Headers.Add("APCA-API-SECRET-KEY", _alpacaConfigSettings.PaperApiSecretKey);
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var account = JsonSerializer.Deserialize<AccountModel>(responseBody);
                _logger.LogInformation("Successfully fetched account information");
                return account;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to fetch account information. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<Order?> GetOrderById(Guid id)
        {
            HttpClient client = _httpClientFactory.CreateClient("AlpacaClient");
            if (client == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }

            var requestUri = $"/v2/orders/{id}";
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            
            request.Headers.Add("Accept", "application/json");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonSerializer.Deserialize<Order>(responseBody);
                _logger.LogInformation("Successfully fetched order with ID: {OrderId}", id);
                return orderResponse;
            }
            else
            {
                string errorContent = response.Content.ReadAsStringAsync().Result;
                _logger.LogError("Failed to fetch order. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<List<Order?>> GetOrdersAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }

            var requestUri = "/v2/orders";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            request.Headers.Add("Accept", "application/json");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody.Equals("[]"))
                {
                    _logger.LogWarning("No Orders to get...");
                    return null;
                }
                var orderResponse = JsonSerializer.Deserialize<List<Order>>(responseBody);
                _logger.LogInformation("Successfully fetched orders");
                return orderResponse;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to fetch orders. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<Order?> PlaceOrderAsync(OrderPlacementRequest orderRequest)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }

            var requestUri = "/v2/orders";

            var payload = new
            {
                symbol = orderRequest.Symbol,
                qty = orderRequest.Quantity,
                side = orderRequest.Side.ToString().ToLower(),
                type = orderRequest.Type.ToString().ToLower(),
                time_in_force = orderRequest.TimeInForce.ToString().ToLower(),
                limit_price = orderRequest.LimitPrice,
                client_order_id = orderRequest.ClientOrderId
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonSerializer.Deserialize<Order>(responseBody);
                _logger.LogInformation("Order placed successfully: {OrderId}", orderResponse?.Id);
                return orderResponse;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to place order. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<AssetsResponse?> GetAssetsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }
            var requestUri = "/v2/assets";
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_alpacaConfigSettings.PaperBaseUrl}{requestUri}");

            request.Headers.Add("Accept", "application/json");

            var response = httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var assets = JsonSerializer.Deserialize<List<Asset>>(responseBody);
                var assetsResponse = new AssetsResponse
                {
                    Assets = assets,
                };
                _logger.LogInformation("Successfully fetched assets");
                return assetsResponse;
            }
            else
            {
                string errorContent = response.Content.ReadAsStringAsync().Result;
                _logger.LogError("Failed to fetch assets. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<List<Position?>> GetPositionsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }

            var requestUri = "/v2/positions";
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            AddHeaders(request);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var positions = JsonSerializer.Deserialize<List<Position>>(responseBody);
                _logger.LogInformation("Successfully fetched positions");
                return positions;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to fetch positions. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<Order> ClosePosition(string symbol, decimal? quantity, decimal? percentage)
        {
            var httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }
            var requestUri = $"/v2/positions/{symbol}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            AddHeaders(request);

            var payload = new
            {
                qty = quantity,
                percentage = percentage
            };
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var closedPosition = JsonSerializer.Deserialize<Order>(responseBody);
                _logger.LogInformation("Successfully closed position for symbol: {Symbol}", symbol);
                return closedPosition;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to close position. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<List<Order?>> ClosePositions(bool cancelOrders)
        {
            var httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }

            var requestUri = $"/v2/positions?cancel_orders={cancelOrders}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            AddHeaders(request);
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var closedPositions = JsonSerializer.Deserialize<List<Order>>(responseBody);
                _logger.LogInformation("Successfully closed all positions");
                return closedPositions;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to close positions. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        public async Task<Position?> GetOpenPositionAsync(string symbol)
        {
            var httpClient = _httpClientFactory.CreateClient("AlpacaClient");
            if (httpClient == null)
            {
                _logger.LogError("Failed to create HTTP client for Alpaca API");
                return null;
            }
            var requestUri = $"/v2/positions/{symbol}";
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            AddHeaders(request);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var position = JsonSerializer.Deserialize<Position>(responseBody);
                _logger.LogInformation("Successfully fetched open position for symbol: {Symbol}", symbol);
                return position;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to fetch open position. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
        }

        private void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("APCA-API-KEY-ID", _alpacaConfigSettings.PaperApiKeyId);
            request.Headers.Add("APCA-API-SECRET-KEY", _alpacaConfigSettings.PaperApiSecretKey);
            request.Headers.Add("Accept", "application/json");
        }
    }
}
