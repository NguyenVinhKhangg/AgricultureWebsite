using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.OrderDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Get all orders with optional pagination and filtering.
        /// If filterParams is null, returns first page with default page size.
        /// </summary>
        Task<PagedResult<OrderListDto>> GetAllOrdersAsync(OrderFilterParams? filterParams = null);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<PagedResult<OrderListDto>> GetOrdersByUserIdAsync(int userId, PaginationParams? paginationParams = null);
        Task<PagedResult<OrderListDto>> GetOrdersByStatusAsync(string status, PaginationParams? paginationParams = null);
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createDto);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> CancelOrderAsync(int orderId);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}