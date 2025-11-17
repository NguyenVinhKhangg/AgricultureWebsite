using AgricultureBackEnd.Data;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AgricultureBackEnd.Repositories.Implement
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByVariantIdAsync(int variantId)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.VariantId == variantId)
                .ToListAsync();
        }
    }
}