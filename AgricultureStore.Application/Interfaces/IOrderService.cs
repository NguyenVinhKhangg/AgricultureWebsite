using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.OrderDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderListDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderListDto>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderListDto>> GetOrdersByStatusAsync(string status);
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createDto);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> CancelOrderAsync(int orderId);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Paginated methods
        Task<PagedResult<OrderListDto>> GetOrdersPagedAsync(OrderFilterParams filterParams);
        Task<PagedResult<OrderListDto>> GetOrdersByUserIdPagedAsync(int userId, PaginationParams paginationParams);
    }
}