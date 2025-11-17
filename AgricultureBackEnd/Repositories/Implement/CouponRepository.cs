using AgricultureBackEnd.Data;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AgricultureBackEnd.Repositories.Implement
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Coupons
                .Where(c => c.IsActive && c.StartDate <= now && c.EndDate >= now)
                .ToListAsync();
        }

        public async Task<bool> ValidateCouponAsync(string code)
        {
            var now = DateTime.UtcNow;
            return await _context.Coupons
                .AnyAsync(c => c.Code == code && 
                              c.IsActive && 
                              c.StartDate <= now && 
                              c.EndDate >= now);
        }

        public async Task<bool> IsCodeUniqueAsync(string code)
        {
            return !await _context.Coupons.AnyAsync(c => c.Code == code);
        }
    }
}