using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Temperance.Agora.Services.Implementations
{
    public class AlpacaLivePriceService : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AlpacaLivePriceService> _logger;
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task? _receiveLoopTask; // Keep a reference to the receive loop task

        public event EventHandler<JsonElement>? OnTradeReceived;
        public event EventHandler<JsonElement>? OnQuoteReceived;
        // It's good practice to also have events for other message types
        public event EventHandler<JsonElement>? OnSubscriptionReceived;
        public event EventHandler<JsonElement>? OnSuccessReceived;
        public event EventHandler<JsonElement>? OnErrorReceived;

        public AlpacaLivePriceService(IConfiguration configuration, ILogger<AlpacaLivePriceService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ConnectAsync(string apiKeyId, string secretKey, string websocketUrl)
        {
            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await _webSocket.ConnectAsync(new Uri(websocketUrl), _cancellationTokenSource.Token);
                _logger.LogInformation("Connected to Alpaca WebSocket API");
                _receiveLoopTask = ReceiveAsync();
                await AuthenticateAsync(apiKeyId, secretKey);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "WebSocket connection error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error during WebSocket connection.");
            }
        }

        public async Task SubscribeToTradesAsync(string symbol)
        {
            await SubscribeAsync(new { action = "subscribe", trades = new[] { symbol } });
        }

        public async Task SubscribeToQuotesAsync(string symbol)
        {
            await SubscribeAsync(new { action = "subscribe", quotes = new[] { symbol } });
        }

        private async Task SubscribeAsync(object subscriptionMessage)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                _logger.LogError("WebSocket is not open for subscription.");
                return;
            }
            var message = JsonSerializer.Serialize(subscriptionMessage);
            await SendMessageAsync(message);
            _logger.LogInformation("Subscription message sent to Alpaca WebSocket API");
        }

        private async Task AuthenticateAsync(string apiKeyId, string secretKey)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                _logger.LogError("WebSocket is not connected.");
                return;
            }
            _logger.LogInformation("Attempting authentication with Key ID: {KeyId}, Secret Key (first 4 chars): {SecretPrefix}", apiKeyId, secretKey?.Substring(0, Math.Min(4, secretKey.Length)));
            var authMessage = JsonSerializer.Serialize(new
            {
                action = "auth",
                key = apiKeyId,
                secret = secretKey
            });

            await SendMessageAsync(authMessage);
            _logger.LogInformation("Authentication message sent to Alpaca WebSocket API");
        }

        private async Task SendMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }

        private async Task ReceiveAsync()
        {
            var buffer = new byte[1024 * 4]; // 4KB buffer
            try
            {
                while (_webSocket != null && _webSocket.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("WebSocket close message received. Status: {CloseStatus}, Description: {Description}", result.CloseStatus, result.CloseStatusDescription);
                        await DisconnectAsync(); // Ensure proper cleanup
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _logger.LogDebug("Received WebSocket message: {Message}", jsonMessage);

                        try
                        {
                            using var document = JsonDocument.Parse(jsonMessage);
                            if (document.RootElement.ValueKind == JsonValueKind.Array) // Alpaca often sends messages in an array
                            {
                                foreach (var element in document.RootElement.EnumerateArray())
                                {
                                    ProcessMessageElement(element);
                                }
                            }
                            else if (document.RootElement.ValueKind == JsonValueKind.Object) // Sometimes a single object
                            {
                                ProcessMessageElement(document.RootElement);
                            }
                            else
                            {
                                _logger.LogWarning("Received WebSocket message that is not a JSON array or object: {Message}", jsonMessage);
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Error parsing JSON message: {Message}", jsonMessage);
                        }
                    }
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely || _cancellationTokenSource.Token.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "WebSocket receive loop ending due to closure or cancellation.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("WebSocket receive loop canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in WebSocket receive loop.");
            }
            finally
            {
                _logger.LogInformation("WebSocket receive loop ended.");
                // Optionally, trigger reconnection logic if the disconnect was not intentional.
            }
        }
        private void ProcessMessageElement(JsonElement element)
        {
            // All Alpaca Data API v2 messages are objects with a "T" (type) property.
            if (!element.TryGetProperty("T", out var messageTypeProperty))
            {
                _logger.LogWarning("Received message element without 'T' property: {MessageElement}", element.ToString());
                return;
            }

            var messageType = messageTypeProperty.GetString();
            _logger.LogInformation("Processing message of type: {MessageType}", messageType);

            switch (messageType)
            {
                case "success": // Authentication successful or other success messages
                    if (element.TryGetProperty("msg", out var successMsg))
                    {
                        _logger.LogInformation("WebSocket Success: {SuccessMessage}", successMsg.GetString());
                        if (successMsg.GetString() == "authenticated")
                        {
                            _logger.LogInformation("Successfully authenticated with Alpaca WebSocket.");
                            // You could raise an event here if needed: OnAuthenticated?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    OnSuccessReceived?.Invoke(this, element.Clone()); // Clone if you need to use it outside the event handler scope
                    break;
                case "error": // Authentication failed or other errors
                    var errorCode = element.TryGetProperty("code", out var codeProp) ? codeProp.GetInt32() : -1;
                    var errorMsgContent = element.TryGetProperty("msg", out var msgProp) ? msgProp.GetString() : "Unknown error";
                    _logger.LogError("WebSocket Error Code {ErrorCode}: {ErrorMessage}", errorCode, errorMsgContent);
                    OnErrorReceived?.Invoke(this, element.Clone());
                    break;
                case "subscription": // Confirmation of subscriptions
                    _logger.LogInformation("Subscription status update: {SubscriptionInfo}", element.ToString());
                    // You can parse further: element.GetProperty("trades").EnumerateArray(), etc.
                    OnSubscriptionReceived?.Invoke(this, element.Clone());
                    break;
                case "t": // Trade update
                    OnTradeReceived?.Invoke(this, element.Clone());
                    break;
                case "q": // Quote update
                    OnQuoteReceived?.Invoke(this, element.Clone());
                    break;
                // Add cases for "b" (bar), "d" (dailyBar), "u" (updatedBar), "s" (tradingStatus) as needed
                default:
                    _logger.LogWarning("Received unknown WebSocket message type: {MessageType} - Content: {MessageContent}", messageType, element.ToString());
                    break;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect requested by client.", _cancellationTokenSource.Token);
                _logger.LogInformation("WebSocket disconnected.");
            }
            _cancellationTokenSource.Cancel();
            _webSocket?.Dispose();
            _webSocket = null;
        }

        public void Dispose()
        {
            DisconnectAsync().Wait();
            _cancellationTokenSource.Dispose();
        }
    }
}
