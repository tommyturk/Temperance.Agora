using Microsoft.AspNetCore.Mvc;
using TradingBot.Agora.Models;
using TradingBot.Agora.Services.Interfaces;

namespace TradingBot.Agora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IAlpacaTradingService _alpacaTradingService;
        private readonly ILogger<PositionsController> _logger;
        public PositionsController(IAlpacaTradingService tradingService, ILogger<PositionsController> logger)
        {
            _alpacaTradingService = tradingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            _logger.LogInformation("Fetching positions from Alpaca API");
            List<Position?> positions = await _alpacaTradingService.GetPositionsAsync();
            if (positions == null)
            {
                _logger.LogError("Failed to fetch positions");
                return NotFound();
            }
            return Ok(positions);
        }

        [HttpGet("open/{symbol}")]
        public async Task<IActionResult> OpenPosition(string symbol)
        {
            _logger.LogInformation("Fetching open position for symbol: {Symbol}", symbol);
            Position? position = await _alpacaTradingService.GetOpenPositionAsync(symbol);
            if (position == null)
            {
                _logger.LogError("Failed to fetch open position for symbol: {Symbol}", symbol);
                return NotFound();
            }
            return Ok(position);
        }

        [HttpDelete("close/{symbol}")]
        public async Task<IActionResult> ClosePosition(string symbol, [FromQuery] decimal? quantity, [FromQuery] decimal? percentage)
        {
            _logger.LogInformation("Closing position for symbol: {Symbol}", symbol);
            Order closedPositionsResponse = await _alpacaTradingService.ClosePosition(symbol, quantity, percentage);
            if (closedPositionsResponse == null)
            {
                _logger.LogError("Failed to close position for symbol: {Symbol}", symbol);
                return NotFound();
            }
            return Ok(closedPositionsResponse);
        }

        [HttpDelete]
        // if cancelOrders is true, cancel all open orders as well as liquidating all positions.
        public async Task<IActionResult> CloseAllPositions([FromQuery] bool cancelOrders)
        {
            _logger.LogInformation("Closing all positions");
            List<Order?> closedPositionsResponse = await _alpacaTradingService.ClosePositions(cancelOrders);
            if (closedPositionsResponse == null)
            {
                _logger.LogError("Failed to close positions");
                return NotFound();
            }
            return Ok(closedPositionsResponse);
        }
    }
}
