using AgricultureBackEnd.Data;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AgricultureBackEnd.Repositories.Implement
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Include(pv => pv.Product)
                .Where(pv => pv.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetWithProductAsync(int variantId)
        {
            return await _context.ProductVariants
                .Include(pv => pv.Product)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(pv => pv.VariantId == variantId);
        }

        public async Task<bool> UpdateStockAsync(int variantId, int quantity)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null || variant.StockQuantity < quantity)
                return false;

            variant.StockQuantity -= quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int threshold)
        {
            return await _context.ProductVariants
                .Include(pv => pv.Product)
                .Where(pv => pv.StockQuantity <= threshold && pv.IsActive)
                .ToListAsync();
        }
    }
}