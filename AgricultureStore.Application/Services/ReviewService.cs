using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.ReviewDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgricultureStore.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<ReviewDto>> GetAllReviewsAsync(ReviewFilterParams? filterParams = null)
        {
            filterParams ??= new ReviewFilterParams();

            _logger.LogDebug("Getting reviews - Page: {PageNumber}, Size: {PageSize}, ProductId: {ProductId}",
                filterParams.PageNumber, filterParams.PageSize, filterParams.ProductId);

            var (reviews, totalCount) = await _unitOfWork.Reviews.GetReviewsPagedAsync(
                filterParams.PageNumber,
                filterParams.PageSize,
                filterParams.ProductId,
                filterParams.UserId,
                filterParams.MinRating,
                filterParams.MaxRating,
                filterParams.SortBy ?? "CreatedAt",
                filterParams.SortDescending);

            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return new PagedResult<ReviewDto>(reviewDtos, totalCount, filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            _logger.LogDebug("Getting review with ID: {ReviewId}", id);
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (review == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} not found", id);
                return null;
            }

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<PagedResult<ReviewDto>> GetReviewsByProductIdAsync(int productId, PaginationParams? paginationParams = null)
        {
            paginationParams ??= new PaginationParams();

            _logger.LogDebug("Getting reviews for product - ProductId: {ProductId}, Page: {PageNumber}",
                productId, paginationParams.PageNumber);

            var (reviews, totalCount) = await _unitOfWork.Reviews.GetByProductIdPagedAsync(
                productId,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return new PagedResult<ReviewDto>(reviewDtos, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<PagedResult<ReviewDto>> GetReviewsByUserIdAsync(int userId, PaginationParams? paginationParams = null)
        {
            paginationParams ??= new PaginationParams();

            _logger.LogDebug("Getting reviews by user - UserId: {UserId}, Page: {PageNumber}", 
                userId, paginationParams.PageNumber);

            var filterParams = new ReviewFilterParams
            {
                UserId = userId,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };

            return await GetAllReviewsAsync(filterParams);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            _logger.LogDebug("Calculating average rating for product: {ProductId}", productId);
            return await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto createDto)
        {
            _logger.LogInformation("Creating review - User: {UserId}, Product: {ProductId}, Rating: {Rating}",
                userId, createDto.ProductId, createDto.Rating);

            if (await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, createDto.ProductId))
            {
                _logger.LogWarning("User {UserId} has already reviewed product {ProductId}", userId, createDto.ProductId);
                throw new InvalidOperationException("You have already reviewed this product");
            }

            var review = _mapper.Map<Review>(createDto);
            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Review created - ReviewId: {ReviewId}, User: {UserId}, Product: {ProductId}",
                review.ReviewId, userId, createDto.ProductId);
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto updateDto)
        {
            _logger.LogDebug("Updating review with ID: {ReviewId}", id);

            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} not found for update", id);
                return false;
            }

            _mapper.Map(updateDto, review);
            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Review updated - ReviewId: {ReviewId}", id);
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            _logger.LogDebug("Deleting review with ID: {ReviewId}", id);

            await _unitOfWork.Reviews.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Review deleted - ReviewId: {ReviewId}", id);
            return true;
        }

        public async Task<bool> CanUserReviewProductAsync(int userId, int productId)
        {
            _logger.LogDebug("Checking if user {UserId} can review product {ProductId}", userId, productId);
            return !await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, productId);
        }
    }
}