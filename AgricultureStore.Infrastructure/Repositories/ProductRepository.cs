using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.ProductName.Contains(searchTerm) && p.IsActive)
                .ToListAsync();
        }

        public async Task<Product?> GetWithVariantsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants.Where(v => v.IsActive))
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product?> GetWithDetailsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}