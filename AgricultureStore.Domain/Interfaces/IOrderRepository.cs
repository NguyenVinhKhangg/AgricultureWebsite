using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task<Order?> GetWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdateStatusAsync(int orderId, string status);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Paginated methods
        Task<(IEnumerable<Order> Items, int TotalCount)> GetOrdersPagedAsync(
            int pageNumber,
            int pageSize,
            string? status = null,
            int? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string sortBy = "OrderDate",
            bool sortDescending = true);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetByUserIdPagedAsync(
            int userId,
            int pageNumber,
            int pageSize);
    }
}
