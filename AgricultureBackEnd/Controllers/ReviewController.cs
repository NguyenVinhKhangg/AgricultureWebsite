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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
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

        [HttpGet("review/user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUserId(int userId)
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(reviews);
        }

        [HttpGet("review/product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByProductId(int productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
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