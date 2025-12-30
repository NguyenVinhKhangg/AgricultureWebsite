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
    }
}
