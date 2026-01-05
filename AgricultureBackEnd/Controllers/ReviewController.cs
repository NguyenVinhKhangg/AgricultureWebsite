using AgricultureStore.Application.DTOs.Common;
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

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Get all reviews with optional pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ReviewDto>>> GetAllReviews([FromQuery] ReviewFilterParams? filterParams)
        {
            var result = await _reviewService.GetAllReviewsAsync(filterParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        /// <summary>
        /// Get reviews by user with optional pagination
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviewsByUserId(
            int userId,
            [FromQuery] PaginationParams? paginationParams)
        {
            var result = await _reviewService.GetReviewsByUserIdAsync(userId, paginationParams);
            return Ok(result);
        }

        /// <summary>
        /// Get reviews by product with optional pagination
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviewsByProductId(
            int productId,
            [FromQuery] PaginationParams? paginationParams)
        {
            var result = await _reviewService.GetReviewsByProductIdAsync(productId, paginationParams);
            return Ok(result);
        }

        [HttpGet("averageRating/{productId}")]
        public async Task<ActionResult<double>> GetAverageRating(int productId)
        {
            var averageRating = await _reviewService.GetAverageRatingAsync(productId);
            return Ok(averageRating);
        }

        [HttpPost("create/{userId}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview(int userId, [FromBody] CreateReviewDto createReviewDto)
        {
            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(userId, createReviewDto);
                return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.ReviewId }, createdReview);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{reviewId}")]
        [Authorize]
        public async Task<ActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto updateReviewDto)
        {
            var result = await _reviewService.UpdateReviewAsync(reviewId, updateReviewDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("delete/{reviewId}")]
        [Authorize]
        public async Task<ActionResult> DeleteReview(int reviewId)
        {
            var result = await _reviewService.DeleteReviewAsync(reviewId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("canReview/{userId}/{productId}")]
        public async Task<ActionResult<bool>> CanUserReviewProduct(int userId, int productId)
        {
            var canReview = await _reviewService.CanUserReviewProductAsync(userId, productId);
            return Ok(canReview);
        }
    }
}