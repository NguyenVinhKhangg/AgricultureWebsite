using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewAsync(int userId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId  == productId)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}