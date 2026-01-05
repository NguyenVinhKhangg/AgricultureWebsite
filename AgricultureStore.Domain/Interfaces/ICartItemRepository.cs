using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int variantId);
        Task<bool> RemoveByUserAndVariantAsync(int userId, int variantId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
    }
}
