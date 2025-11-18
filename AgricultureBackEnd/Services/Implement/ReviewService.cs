using AgricultureBackEnd.DTOs.ReviewDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
        {
            var reviews = await _unitOfWork.Reviews.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto createDto)
        {
            // Check if user has already reviewed this product
            if (await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, createDto.ProductId))
                throw new InvalidOperationException("You have already reviewed this product");

            var review = _mapper.Map<Review>(createDto);
            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto updateDto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) return false;

            _mapper.Map(updateDto, review);
            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            await _unitOfWork.Reviews.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserReviewProductAsync(int userId, int productId)
        {
            return !await _unitOfWork.Reviews.HasUserReviewedProductAsync(userId, productId);
        }
    }
}