using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class UserAddressRepository : Repository<UserAddress>, IUserAddressRepository
    {
        public UserAddressRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId)
        {
            return await _context.UserAddresses
                .Where(ua => ua.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserAddress?> GetDefaultAddressAsync(int userId)
        {
            return await _context.UserAddresses
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.IsDefault);
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            // Remove default from all user's addresses
            var userAddresses = await _context.UserAddresses
                .Where(ua => ua.UserId == userId)
                .ToListAsync();

            foreach (var address in userAddresses)
            {
                address.IsDefault = address.AddressId == addressId;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}