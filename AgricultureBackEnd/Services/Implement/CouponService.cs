using AgricultureBackEnd.DTOs.CouponDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CouponService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _unitOfWork.Coupons.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);
            return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
        }

        public async Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
        {
            var coupons = await _unitOfWork.Coupons.GetActiveCouponsAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto)
        {
            if (!await _unitOfWork.Coupons.IsCodeUniqueAsync(createDto.Code))
                throw new InvalidOperationException("Coupon code already exists");

            var coupon = _mapper.Map<Coupon>(createDto);
            await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<bool> UpdateCouponAsync(int id, UpdateCouponDto updateDto)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null) return false;

            _mapper.Map(updateDto, coupon);
            await _unitOfWork.Coupons.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null) return false;

            coupon.IsActive = false;
            await _unitOfWork.Coupons.UpdateAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateCouponAsync(string code)
        {
            return await _unitOfWork.Coupons.ValidateCouponAsync(code);
        }

        public async Task<decimal> CalculateDiscountAsync(string code, decimal orderAmount)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);
            if (coupon == null || !await ValidateCouponAsync(code))
                return 0;

            return coupon.DiscountValue;
        }
    }
}