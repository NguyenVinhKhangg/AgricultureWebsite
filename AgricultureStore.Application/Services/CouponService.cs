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
            _logger.LogDebug("Getting all coupons");
            var coupons = await _unitOfWork.Coupons.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int id)
        {
            _logger.LogDebug("Getting coupon with ID: {CouponId}", id);
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);

            if (coupon == null)
            {
                _logger.LogWarning("Coupon with ID {CouponId} not found", id);
                return null;
            }

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            _logger.LogDebug("Getting coupon by code: {Code}", code);
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);

            if (coupon == null)
            {
                _logger.LogWarning("Coupon with code {Code} not found", code);
                return null;
            }

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
        {
            _logger.LogDebug("Getting active coupons");
            var coupons = await _unitOfWork.Coupons.GetActiveCouponsAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto)
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

            _logger.LogInformation("Coupon created - CouponId: {CouponId}, Code: {Code}", coupon.CouponId, coupon.Code);
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<bool> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
        {
            _logger.LogDebug("Updating coupon with ID: {CouponId}", id);

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
            {
                _logger.LogWarning("Coupon with ID {CouponId} not found for update", id);
                return false;
            }

            _mapper.Map(updateDto, coupon);
            await _unitOfWork.Coupons.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Coupon updated - CouponId: {CouponId}", id);
            return true;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            _logger.LogDebug("Soft deleting coupon with ID: {CouponId}", id);

            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
            {
                _logger.LogWarning("Coupon with ID {CouponId} not found for deletion", id);
                return false;
            }

            coupon.IsActive = false;
            await _unitOfWork.Coupons.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Coupon soft-deleted - CouponId: {CouponId}", id);
            return true;
        }

        public async Task<bool> ValidateCouponAsync(string code)
        {
            _logger.LogDebug("Validating coupon: {Code}", code);
            var isValid = await _unitOfWork.Coupons.ValidateCouponAsync(code);

            if (!isValid)
            {
                _logger.LogWarning("Coupon {Code} is invalid or expired", code);
            }

            return isValid;
        }

        public async Task<decimal> CalculateDiscountAsync(string code, decimal orderAmount)
        {
            _logger.LogDebug("Calculating discount for coupon {Code}, OrderAmount: {Amount}", code, orderAmount);

            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);
            if (coupon == null)
            {
                _logger.LogWarning("Coupon {Code} not found for discount calculation", code);
                return 0;
            }

            var isValid = await ValidateCouponAsync(code);
            if (!isValid)
            {
                return 0;
            }

            return coupon.DiscountValue;
        }
    }
}