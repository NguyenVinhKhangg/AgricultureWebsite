using AgricultureBackEnd.DTOs.ReviewDTOs;

namespace AgricultureBackEnd.Services.Interface
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId);
        Task<double> GetAverageRatingAsync(int productId);
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto createDto);
        Task<bool> UpdateReviewAsync(int id, UpdateReviewDto updateDto);
        Task<bool> DeleteReviewAsync(int id);
        Task<bool> CanUserReviewProductAsync(int userId, int productId);
    }
}