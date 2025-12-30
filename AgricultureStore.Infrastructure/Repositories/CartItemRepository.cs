using AgricultureStore.Infrastructure.Data;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId)
        {
            return await _context.CartItems
                .Include(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int userId, int variantId)
        {
            return await _context.CartItems
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.VariantId == variantId);
        }

        public async Task<bool> RemoveByUserAndVariantAsync(int userId, int variantId)
        {
            var cartItem = await GetCartItemAsync(userId, variantId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return false;

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }
    }
}