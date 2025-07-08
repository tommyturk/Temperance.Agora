using Microsoft.AspNetCore.Mvc;
using Temperance.Agora.Services.Interfaces;

namespace Temperance.Agora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private readonly IStockService _stockService;

        private readonly ILogger<StockController> _logger;
        public StockController(IStockService stockService, ILogger<StockController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet("latest-quote/{symbol}")]
        public async Task<IActionResult> GetLatestQuote(string symbol)
        {
            _logger.LogInformation("Fetching latest quote for {Symbol}", symbol);
            var latestQuote = await _stockService.GetLatestStockQuoteAsync(symbol);
            if (latestQuote == null)
            {
                _logger.LogError("Failed to fetch latest quote for {Symbol}", symbol);
                return NotFound();
            }
            return Ok(latestQuote);
        }
    }
}
