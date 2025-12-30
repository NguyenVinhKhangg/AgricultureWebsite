using AgricultureStore.Application.DTOs.CouponDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
namespace AgricultureStore.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponService> _logger;

        public CouponService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CouponService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CouponDto>> GetAllCouponsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all coupons");
                var coupons = await _unitOfWork.Coupons.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} coupons", coupons.Count());
                return _mapper.Map<IEnumerable<CouponDto>>(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all coupons");
                throw;
            }
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting coupon with ID: {CouponId}", id);
                var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

                if (coupon == null)
                {
                    _logger.LogWarning("Coupon with ID {CouponId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved coupon: {Code}", coupon.Code);
                return _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coupon with ID: {CouponId}", id);
                throw;
            }
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            try
            {
                _logger.LogInformation("Getting coupon by code: {Code}", code);
                var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);

                if (coupon == null)
                {
                    _logger.LogWarning("Coupon with code {Code} not found", code);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved coupon: {Code}", code);
                return _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coupon by code: {Code}", code);
                throw;
            }
        }

        public async Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
        {
            try
            {
                _logger.LogInformation("Getting active coupons");
                var coupons = await _unitOfWork.Coupons.GetActiveCouponsAsync();
                _logger.LogInformation("Retrieved {Count} active coupons", coupons.Count());
                return _mapper.Map<IEnumerable<CouponDto>>(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active coupons");
                throw;
            }
        }

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new coupon: {Code}", createDto.Code);

                if (!await _unitOfWork.Coupons.IsCodeUniqueAsync(createDto.Code))
                {
                    _logger.LogWarning("Coupon code {Code} already exists", createDto.Code);
                    throw new InvalidOperationException("Coupon code already exists");
                }

                var coupon = _mapper.Map<Coupon>(createDto);
                await _unitOfWork.Coupons.AddAsync(coupon);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Coupon created successfully: {Code} with ID: {CouponId}", coupon.Code, coupon.CouponId);
                return _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating coupon: {Code}", createDto.Code);
                throw;
            }
        }

        public async Task<bool> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating coupon with ID: {CouponId}", id);

                var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon with ID {CouponId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, coupon);
                await _unitOfWork.Coupons.UpdateAsync(coupon);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Coupon updated successfully: {CouponId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coupon with ID: {CouponId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            try
            {
                _logger.LogInformation("Soft deleting coupon with ID: {CouponId}", id);

                var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon with ID {CouponId} not found for deletion", id);
                    return false;
                }

                coupon.IsActive = false;
                await _unitOfWork.Coupons.UpdateAsync(coupon);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Coupon soft-deleted successfully: {CouponId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting coupon with ID: {CouponId}", id);
                throw;
            }
        }

        public async Task<bool> ValidateCouponAsync(string code)
        {
            try
            {
                _logger.LogInformation("Validating coupon: {Code}", code);
                var isValid = await _unitOfWork.Coupons.ValidateCouponAsync(code);

                if (isValid)
                {
                    _logger.LogInformation("Coupon {Code} is valid", code);
                }
                else
                {
                    _logger.LogWarning("Coupon {Code} is invalid or expired", code);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon: {Code}", code);
                throw;
            }
        }

        public async Task<decimal> CalculateDiscountAsync(string code, decimal orderAmount)
        {
            try
            {
                _logger.LogInformation("Calculating discount for coupon {Code}, Order Amount: {Amount:C}", code, orderAmount);

                var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon {Code} not found for discount calculation", code);
                    return 0;
                }

                var isValid = await ValidateCouponAsync(code);
                if (!isValid)
                {
                    _logger.LogWarning("Coupon {Code} is not valid for discount calculation", code);
                    return 0;
                }

                var discount = coupon.DiscountValue;
                _logger.LogInformation("Discount calculated - Code: {Code}, Order: {OrderAmount:C}, Discount: {Discount:C}",
                    code, orderAmount, discount);

                return discount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating discount for coupon: {Code}", code);
                throw;
            }
        }
    }
}