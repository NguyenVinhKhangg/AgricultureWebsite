using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
        Task<Review?> GetReviewAsync(int userId, int productId);
        Task<double> GetAverageRatingAsync(int productId);
        Task<bool> HasUserReviewedProductAsync(int userId, int productId);

        // Paginated methods
        Task<(IEnumerable<Review> Items, int TotalCount)> GetReviewsPagedAsync(
            int pageNumber,
            int pageSize,
            int? productId = null,
            int? userId = null,
            int? minRating = null,
            int? maxRating = null,
            string sortBy = "CreatedAt",
            bool sortDescending = true);

        Task<(IEnumerable<Review> Items, int TotalCount)> GetByProductIdPagedAsync(
            int productId,
            int pageNumber,
            int pageSize);
    }
}
