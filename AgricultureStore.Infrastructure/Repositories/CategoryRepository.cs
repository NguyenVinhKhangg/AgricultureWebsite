using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();
        }

        public async Task<Category?> GetWithSubCategoriesAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .ToListAsync();
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _context.Products
                .AnyAsync(p => p.CategoryId == categoryId);
        }
    }
}