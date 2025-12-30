using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderDetail>> GetByVariantIdAsync(int variantId);
    }
}
