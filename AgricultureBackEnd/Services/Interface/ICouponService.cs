using AgricultureBackEnd.DTOs.CouponDTOs;

namespace AgricultureBackEnd.Services.Interface
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetAllCouponsAsync();
        Task<CouponDto?> GetCouponByIdAsync(int id);
        Task<CouponDto?> GetCouponByCodeAsync(string code);
        Task<IEnumerable<CouponDto>> GetActiveCouponsAsync();
        Task<CouponDto> CreateCouponAsync(CreateCouponDto createDto);
        Task<bool> UpdateCouponAsync(int id, UpdateCouponDto updateDto);
        Task<bool> DeleteCouponAsync(int id);
        Task<bool> ValidateCouponAsync(string code);
        Task<decimal> CalculateDiscountAsync(string code, decimal orderAmount);
    }
}