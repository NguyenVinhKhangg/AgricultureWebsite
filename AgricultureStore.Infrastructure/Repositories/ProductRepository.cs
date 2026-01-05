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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.ProductName.Contains(searchTerm) && p.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetWithVariantsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants.Where(v => v.IsActive))
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product?> GetWithDetailsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .AsSplitQuery() // Avoid cartesian explosion
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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "CreatedAt",
            bool sortDescending = true)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive)
                .AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm) || 
                                        (p.Description != null && p.Description.Contains(searchTerm)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.ProductVariants.Any(v => v.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.ProductVariants.Any(v => v.Price <= maxPrice.Value));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => sortDescending 
                    ? query.OrderByDescending(p => p.ProductName) 
                    : query.OrderBy(p => p.ProductName),
                "price" => sortDescending 
                    ? query.OrderByDescending(p => p.ProductVariants.Min(v => v.Price)) 
                    : query.OrderBy(p => p.ProductVariants.Min(v => v.Price)),
                _ => sortDescending 
                    ? query.OrderByDescending(p => p.CreatedAt) 
                    : query.OrderBy(p => p.CreatedAt)
            };

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetByCategoryPagedAsync(
            int categoryId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}