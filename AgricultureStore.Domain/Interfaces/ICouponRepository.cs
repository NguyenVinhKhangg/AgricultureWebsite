using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();
        Task<bool> ValidateCouponAsync(string code);
        Task<bool> IsCodeUniqueAsync(string code);
    }
}
