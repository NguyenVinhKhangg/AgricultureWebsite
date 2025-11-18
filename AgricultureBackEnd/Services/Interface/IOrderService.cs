using AgricultureBackEnd.DTOs.OrderDTOs;

namespace AgricultureBackEnd.Services.Interface
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
    }
}