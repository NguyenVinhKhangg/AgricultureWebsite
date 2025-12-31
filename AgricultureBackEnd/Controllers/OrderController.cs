using AgricultureStore.Application.DTOs.OrderDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả orders
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Received request to get all orders");
                var orders = await _orderService.GetAllOrdersAsync();
                _logger.LogInformation("Returning {Count} orders", orders.Count());
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all orders");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy order theo ID với details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                _logger.LogInformation("Received request to get order with ID {Id}", id);
                var order = await _orderService.GetOrderByIdAsync(id);
                
                if (order == null)
                {
                    _logger.LogWarning("Order with ID {Id} not found", id);
                    return NotFound($"Order with ID {id} not found");
                }
                
                _logger.LogInformation("Returning order with ID {Id}", id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting order with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy orders theo user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetOrdersByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("Received request to get orders for user ID {UserId}", userId);
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                _logger.LogInformation("Returning {Count} orders for user ID {UserId}", orders.Count(), userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting orders for user ID {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy orders theo status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetOrdersByStatus(string status)
        {
            try
            {
                _logger.LogInformation("Received request to get orders with status {Status}", status);
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                _logger.LogInformation("Returning {Count} orders with status {Status}", orders.Count(), status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting orders with status {Status}", status);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tạo order mới từ cart
        /// </summary>
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<OrderDto>> CreateOrder(int userId, [FromBody] CreateOrderDto createDto)
        {
            try
            {
                _logger.LogInformation("Received request to create order for user ID {UserId}", userId);
                var order = await _orderService.CreateOrderAsync(userId, createDto);
                _logger.LogInformation("Created order with ID {OrderId} for user {UserId}", order.OrderId, userId);
                
                return CreatedAtAction(
                    nameof(GetOrderById), 
                    new { id = order.OrderId }, 
                    order
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating order for user {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order for user ID {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cập nhật status của order
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                _logger.LogInformation("Received request to update status for order ID {OrderId} to {Status}", id, statusDto.Status);
                var result = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
                
                if (!result)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found for status update", id);
                    return NotFound($"Order with ID {id} not found");
                }
                
                _logger.LogInformation("Successfully updated status for order ID {OrderId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for order ID {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Hủy order
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                _logger.LogInformation("Received request to cancel order ID {OrderId}", id);
                var result = await _orderService.CancelOrderAsync(id);
                
                if (!result)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found or cannot be cancelled", id);
                    return NotFound($"Order with ID {id} not found or cannot be cancelled");
                }
                
                _logger.LogInformation("Successfully cancelled order ID {OrderId}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot cancel order ID {OrderId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling order ID {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy tổng doanh thu
        /// </summary>
        [HttpGet("revenue")]
        public async Task<ActionResult<decimal>> GetTotalRevenue(
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation(
                    "Received request to get total revenue from {StartDate} to {EndDate}", 
                    startDate, 
                    endDate
                );
                
                var revenue = await _orderService.GetTotalRevenueAsync(startDate, endDate);
                
                _logger.LogInformation("Total revenue: {Revenue:C}", revenue);
                
                return Ok(new { 
                    revenue = revenue,
                    startDate = startDate,
                    endDate = endDate,
                    currency = "VND"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total revenue");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy thống kê orders theo ngày
        /// </summary>
        [HttpGet("statistics/daily")]
        public async Task<ActionResult> GetDailyStatistics([FromQuery] DateTime date)
        {
            try
            {
                _logger.LogInformation("Received request to get daily statistics for {Date}", date);
                
                var orders = await _orderService.GetAllOrdersAsync();
                var dailyOrders = orders.Where(o => o.OrderDate.Date == date.Date);
                
                var statistics = new
                {
                    date = date.Date,
                    totalOrders = dailyOrders.Count(),
                    totalRevenue = dailyOrders.Sum(o => o.TotalAmount),
                    pendingOrders = dailyOrders.Count(o => o.Status == "Pending"),
                    completedOrders = dailyOrders.Count(o => o.Status == "Completed"),
                    cancelledOrders = dailyOrders.Count(o => o.Status == "Cancelled")
                };
                
                _logger.LogInformation("Returning daily statistics for {Date}", date);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting daily statistics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}