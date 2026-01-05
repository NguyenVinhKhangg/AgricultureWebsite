using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.ReviewDTOs;

namespace AgricultureStore.Application.Interfaces
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

        // Paginated methods
        Task<PagedResult<ReviewDto>> GetReviewsPagedAsync(ReviewFilterParams filterParams);
        Task<PagedResult<ReviewDto>> GetReviewsByProductIdPagedAsync(int productId, PaginationParams paginationParams);
    }
}