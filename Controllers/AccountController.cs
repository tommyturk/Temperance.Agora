using Microsoft.AspNetCore.Mvc;
using Temperance.Agora.Services.Interfaces;

namespace Temperance.Agora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAlpacaTradingService _tradingService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAlpacaTradingService tradingService, ILogger<AccountController> logger)
        {
            _tradingService = tradingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            _logger.LogInformation("Fetching account information from Alpaca API");
            var account = await _tradingService.GetAccountAsync();
            if (account == null)
            {
                _logger.LogError("Failed to fetch account information");
                return NotFound();
            }

            return Ok(account);
        }
    }
}
