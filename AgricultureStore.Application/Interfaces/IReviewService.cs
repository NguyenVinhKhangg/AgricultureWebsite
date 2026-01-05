using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.ReviewDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IReviewService
    {
        /// <summary>
        /// Get all reviews with optional pagination and filtering.
        /// </summary>
        Task<PagedResult<ReviewDto>> GetAllReviewsAsync(ReviewFilterParams? filterParams = null);
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<PagedResult<ReviewDto>> GetReviewsByProductIdAsync(int productId, PaginationParams? paginationParams = null);
        Task<PagedResult<ReviewDto>> GetReviewsByUserIdAsync(int userId, PaginationParams? paginationParams = null);
        Task<double> GetAverageRatingAsync(int productId);
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto createDto);
        Task<bool> UpdateReviewAsync(int id, UpdateReviewDto updateDto);
        Task<bool> DeleteReviewAsync(int id);
        Task<bool> CanUserReviewProductAsync(int userId, int productId);
    }
}