using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.ReviewDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
        {
            _logger.LogInformation("Received request to get all reviews");
            var reviews = await _reviewService.GetAllReviewsAsync();
            _logger.LogInformation("Returning {Count} reviews", reviews.Count());
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
        {
            _logger.LogInformation("Received request to get review with ID {Id}", id);
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review with ID {Id} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Returning review with ID {Id}", id);
            return Ok(review);
        }


        [HttpGet("review/user/{userId}")]
        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserId(int userId)
        {
            _logger.LogInformation("Received request to get reviews by user ID {UserId}", userId);
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            _logger.LogInformation("Returning {Count} reviews for user ID {UserId}", reviews.Count(), userId);
            return reviews;
        }

        [HttpGet("review/product/{productId}")]
        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductId(int productId)
        {
            _logger.LogInformation("Received request to get reviews for product ID {ProductId}", productId);
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            _logger.LogInformation("Returning {Count} reviews for product ID {ProductId}", reviews.Count(), productId);
            return reviews;
        }

        [HttpGet("averageRating/{productId}")]
        public async Task<ActionResult<double>> GetAverageRating(int productId)
        {
            _logger.LogInformation("Received request to get average rating for product ID {ProductId}", productId);
            var averageRating = await _reviewService.GetAverageRatingAsync(productId);
            _logger.LogInformation("Returning average rating {AverageRating} for product ID {ProductId}", averageRating, productId);
            return Ok(averageRating);
        }

        [HttpPost("create/{userId}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview(int userId, [FromBody] CreateReviewDto createReviewDto)
        {
            _logger.LogInformation("Received request to create a new review");
            var createdReview = await _reviewService.CreateReviewAsync(userId,createReviewDto);
         
            _logger.LogInformation("Created review with ID {ReviewId}", createdReview.ReviewId);
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.ReviewId }, createdReview);
        }

        [HttpPut("update/{reviewId}")]
        [Authorize]
        public async Task<ActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto updateReviewDto)
        {
            _logger.LogInformation("Received request to update review with ID {ReviewId}", reviewId);
            var result = await _reviewService.UpdateReviewAsync(reviewId, updateReviewDto);
            if (!result)
            {
                _logger.LogWarning("Failed to update review with ID {ReviewId}", reviewId);
                return NotFound();
            }
            _logger.LogInformation("Successfully updated review with ID {ReviewId}", reviewId);
            return NoContent();
        }

        [HttpDelete("delete/{reviewId}")]
        [Authorize]
        public async Task<ActionResult> DeleteReview(int reviewId)
        {
            _logger.LogInformation("Received request to delete review with ID {ReviewId}", reviewId);
            var result = await _reviewService.DeleteReviewAsync(reviewId);
            if (!result)
            {
                _logger.LogWarning("Failed to delete review with ID {ReviewId}", reviewId);
                return NotFound();
            }
            _logger.LogInformation("Successfully deleted review with ID {ReviewId}", reviewId);
            return NoContent();
        }

        [HttpGet("canReview/{userId}/{productId}")]
        public async Task<ActionResult<bool>> CanUserReviewProduct(int userId, int productId)
        {
            _logger.LogInformation("Received request to check if user ID {UserId} can review product ID {ProductId}", userId, productId);
            var canReview = await _reviewService.CanUserReviewProductAsync(userId, productId);
            _logger.LogInformation("User ID {UserId} can review product ID {ProductId}: {CanReview}", userId, productId, canReview);
            return Ok(canReview);
        }
    }
}