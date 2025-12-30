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

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all reviews");
                var reviews = await _unitOfWork.Reviews.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} reviews", reviews.Count());
                return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                throw;
            }
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting review with ID: {ReviewId}", id);
                var review = await _unitOfWork.Reviews.GetByIdAsync(id);

                if (review == null)
                {
                    _logger.LogWarning("Review with ID {ReviewId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved review: {ReviewId}", id);
                return _mapper.Map<ReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review with ID: {ReviewId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            try
            {
                _logger.LogInformation("Getting reviews for product: {ProductId}", productId);
                var reviews = await _unitOfWork.Reviews.GetByProductIdAsync(productId);
                _logger.LogInformation("Retrieved {Count} reviews for product {ProductId}", reviews.Count(), productId);
                return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Getting reviews by user: {UserId}", userId);
                var reviews = await _unitOfWork.Reviews.GetByUserIdAsync(userId);
                _logger.LogInformation("Retrieved {Count} reviews by user {UserId}", reviews.Count(), userId);
                return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews by user: {UserId}", userId);
                throw;
            }
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            try
            {
                _logger.LogInformation("Calculating average rating for product: {ProductId}", productId);
                var average = await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
                _logger.LogInformation("Average rating for product {ProductId}: {Rating}", productId, average);
                return average;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average rating for product: {ProductId}", productId);
                throw;
            }
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto createDto)
        {
            try
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

                _logger.LogInformation("Review created successfully - ID: {ReviewId}", review.ReviewId);
                return _mapper.Map<ReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review - User: {UserId}, Product: {ProductId}", userId, createDto.ProductId);
                throw;
            }
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating review with ID: {ReviewId}", id);

                var review = await _unitOfWork.Reviews.GetByIdAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID {ReviewId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, review);
                await _unitOfWork.Reviews.UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review updated successfully: {ReviewId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review with ID: {ReviewId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting review with ID: {ReviewId}", id);

                await _unitOfWork.Reviews.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review deleted successfully: {ReviewId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID: {ReviewId}", id);
                throw;
            }
        }

        public async Task<bool> CanUserReviewProductAsync(int userId, int productId)
        {
            try
            {
                _logger.LogDebug("Checking if user {UserId} can review product {ProductId}", userId, productId);
                return !await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking review eligibility - User: {UserId}, Product: {ProductId}", userId, productId);
                throw;
            }
        }
    }
}