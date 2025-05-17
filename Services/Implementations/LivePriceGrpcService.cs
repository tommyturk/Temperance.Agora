using Grpc.Core;
using Temperance.Agora.Settings;

namespace Temperance.Agora.Services.Implementations
{
    public class LivePriceGrpcService : LivePrice.LivePriceBase
    {
        private readonly AlpacaLivePriceService _alpacaLivePriceService;
        private readonly AlpacaConfigSettings _alpacaConfigSettings;
        private readonly ILogger<LivePriceGrpcService> _logger;

        public LivePriceGrpcService(AlpacaLivePriceService alpacaLivePriceService, AlpacaConfigSettings alpacaConfigSettings,
            ILogger<LivePriceGrpcService> logger)
        {
            _alpacaLivePriceService = alpacaLivePriceService;
            _alpacaConfigSettings = alpacaConfigSettings;
            _logger = logger;
        }

        public override async Task<ConnectResponse> ConnectAndSubscribe(ConnectRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received ConnectAndSubscribe reuest for symbols: {Symbols}", string.Join(", ", request.Symbols));

            var apiKeyId = _alpacaConfigSettings.PaperApiKeyId;
            var secretKey = _alpacaConfigSettings.PaperApiSecretKey;
            var websocketUrl = _alpacaConfigSettings.PaperWebSocketUrl;

            if (string.IsNullOrEmpty(apiKeyId) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(websocketUrl))
            {
                _logger.LogError("Alpaca API credentials are not set.");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Alpaca API credentials or WebSocket URL not configured."));
            }

            try
            {
                await _alpacaLivePriceService.ConnectAsync(apiKeyId, secretKey, websocketUrl);
                foreach(var symbol in request.Symbols)
                {
                    await _alpacaLivePriceService.SubscribeToTradesAsync(symbol);
                    await _alpacaLivePriceService.SubscribeToQuotesAsync(symbol);
                    _logger.LogInformation("Subscribed to trade and quote updates for symbol: {Symbol}", symbol);   
                }
                return new ConnectResponse { Success = true, Message = "Connected and subscribed to symbols." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Alpaca WebSocket API.");
                return new ConnectResponse { Success = false, Message = "Error connecting to Alpaca WebSocket API." };
            }
        }
    }
}
