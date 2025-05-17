using Microsoft.AspNetCore.Mvc;
using Temperance.Agora.Models;
using Temperance.Agora.Services.Interfaces;

namespace Temperance.Agora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IAlpacaTradingService _alpacaTradingService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IAlpacaTradingService tradingService, ILogger<OrdersController> logger)
        {
            _alpacaTradingService = tradingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            _logger.LogInformation("Fetching orders from Alpaca API");
            var orders = await _alpacaTradingService.GetOrdersAsync();
            if (orders == null)
            {
                _logger.LogError("Failed to fetch orders");
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderPlacementRequest orderRequest)
        {
            _logger.LogInformation("Received request to create order: Symbol={Symbol}, Qty={Quantity}, Side={Side}, Type={Type}",
                orderRequest.Symbol, orderRequest.Quantity, orderRequest.Side, orderRequest.Type);

            var submittedOrder = await _alpacaTradingService.PlaceOrderAsync(orderRequest);

            if (submittedOrder == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create order. Check logs for details.");

            return Ok(submittedOrder);
        }

        [HttpGet("orderById")]
        public async Task<IActionResult> GetOrderById([FromQuery] Guid id)
        {
            Order? order = await _alpacaTradingService.GetOrderById(id);

            if(order == null)
            {
                _logger.LogInformation($"Could not find order with order Id: {id}");
                return StatusCode(StatusCodes.Status204NoContent, $"Could not find order with order Id: {id}");
            }

            return Ok(order);
        }

        // Cancel an open order

        // Replace an order
    }
}
