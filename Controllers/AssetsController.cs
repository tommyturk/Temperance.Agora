using Microsoft.AspNetCore.Mvc;
using TradingBot.Agora.Models;
using TradingBot.Agora.Services.Interfaces;

namespace TradingBot.Agora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAlpacaTradingService _alpacaTradingService;

        private readonly ILogger<AssetsController> _logger;

        public AssetsController(IAlpacaTradingService tradingService, ILogger<AssetsController> logger)
        {
            _alpacaTradingService = tradingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Assets()
        {
            _logger.LogInformation("Fetching assets from Alpaca API");
            AssetsResponse assets = await _alpacaTradingService.GetAssetsAsync();
            if (assets == null)
            {
                _logger.LogError("Failed to fetch assets");
                return NotFound();
            }
            return Ok(assets);
        }
    }
}
