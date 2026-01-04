using AgricultureStore.Application.DTOs.OrderDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Lấy tất cả orders
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Lấy order theo ID với details
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found");
            }
            
            return Ok(order);
        }

        /// <summary>
        /// Lấy orders theo user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        /// <summary>
        /// Lấy orders theo status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderListDto>>> GetOrdersByStatus(string status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        /// <summary>
        /// Tạo order mới từ cart
        /// </summary>
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<OrderDto>> CreateOrder(int userId, [FromBody] CreateOrderDto createDto)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(userId, createDto);
                
                return CreatedAtAction(
                    nameof(GetOrderById), 
                    new { id = order.OrderId }, 
                    order
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật status của order
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
            
            if (!result)
            {
                return NotFound($"Order with ID {id} not found");
            }
            
            return NoContent();
        }

        /// <summary>
        /// Hủy order
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                
                if (!result)
                {
                    return NotFound($"Order with ID {id} not found or cannot be cancelled");
                }
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
            var revenue = await _orderService.GetTotalRevenueAsync(startDate, endDate);
            
            return Ok(new { 
                revenue = revenue,
                startDate = startDate,
                endDate = endDate,
                currency = "VND"
            });
        }

        /// <summary>
        /// Lấy thống kê orders theo ngày
        /// </summary>
        [HttpGet("statistics/daily")]
        public async Task<ActionResult> GetDailyStatistics([FromQuery] DateTime date)
        {
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
            
            return Ok(statistics);
        }
    }
}