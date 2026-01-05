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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Review?> GetReviewAsync(int userId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            // Use database-side calculation for better performance
            var avgRating = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();

            return avgRating ?? 0;
        }

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetReviewsPagedAsync(
            int pageNumber,
            int pageSize,
            int? productId = null,
            int? userId = null,
            int? minRating = null,
            int? maxRating = null,
            string sortBy = "CreatedAt",
            bool sortDescending = true)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .AsNoTracking()
                .AsQueryable();

            // Apply filters
            if (productId.HasValue)
            {
                query = query.Where(r => r.ProductId == productId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(r => r.UserId == userId.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(r => r.Rating >= minRating.Value);
            }

            if (maxRating.HasValue)
            {
                query = query.Where(r => r.Rating <= maxRating.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "rating" => sortDescending 
                    ? query.OrderByDescending(r => r.Rating) 
                    : query.OrderBy(r => r.Rating),
                _ => sortDescending 
                    ? query.OrderByDescending(r => r.CreatedAt) 
                    : query.OrderBy(r => r.CreatedAt)
            };

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetByProductIdPagedAsync(
            int productId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}