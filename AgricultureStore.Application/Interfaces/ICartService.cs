using AgricultureStore.Application.DTOs.CartDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetCartItemsAsync(int userId);
        Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<bool> UpdateCartItemAsync(int userId, int variantId, int quantity);
        Task<bool> RemoveFromCartAsync(int userId, int variantId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
    }
}