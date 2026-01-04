using AgricultureStore.Application.DTOs.CartDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgricultureStore.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(int userId)
        {
            _logger.LogDebug("Getting cart items for user: {UserId}", userId);
            var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
        }

        public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            _logger.LogDebug("Adding item to cart - User: {UserId}, Variant: {VariantId}, Quantity: {Quantity}",
                userId, addToCartDto.VariantId, addToCartDto.Quantity);

            var existingItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, addToCartDto.VariantId);

            if (existingItem != null)
            {
                existingItem.Quantity += addToCartDto.Quantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cart item quantity updated - User: {UserId}, Variant: {VariantId}, NewQuantity: {Quantity}",
                    userId, addToCartDto.VariantId, existingItem.Quantity);
                return _mapper.Map<CartItemDto>(existingItem);
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                VariantId = addToCartDto.VariantId,
                Quantity = addToCartDto.Quantity
            };

            await _unitOfWork.CartItems.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item added to cart - User: {UserId}, Variant: {VariantId}", userId, addToCartDto.VariantId);

            var addedItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, addToCartDto.VariantId);
            return _mapper.Map<CartItemDto>(addedItem);
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int variantId, int quantity)
        {
            _logger.LogDebug("Updating cart item - User: {UserId}, Variant: {VariantId}, Quantity: {Quantity}",
                userId, variantId, quantity);

            var cartItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, variantId);
            if (cartItem == null)
            {
                _logger.LogWarning("Cart item not found - User: {UserId}, Variant: {VariantId}", userId, variantId);
                return false;
            }

            if (quantity <= 0)
            {
                return await RemoveFromCartAsync(userId, variantId);
            }

            cartItem.Quantity = quantity;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cart item updated - User: {UserId}, Variant: {VariantId}, Quantity: {Quantity}",
                userId, variantId, quantity);
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int variantId)
        {
            _logger.LogDebug("Removing item from cart - User: {UserId}, Variant: {VariantId}", userId, variantId);
            var result = await _unitOfWork.CartItems.RemoveByUserAndVariantAsync(userId, variantId);

            if (result)
            {
                _logger.LogInformation("Item removed from cart - User: {UserId}, Variant: {VariantId}", userId, variantId);
            }
            else
            {
                _logger.LogWarning("Failed to remove item from cart - not found - User: {UserId}, Variant: {VariantId}", userId, variantId);
            }

            return result;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            _logger.LogDebug("Clearing cart for user: {UserId}", userId);
            var result = await _unitOfWork.CartItems.ClearCartAsync(userId);

            if (result)
            {
                _logger.LogInformation("Cart cleared - UserId: {UserId}", userId);
            }

            return result;
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            _logger.LogDebug("Getting cart item count for user: {UserId}", userId);
            return await _unitOfWork.CartItems.GetCartItemCountAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            _logger.LogDebug("Calculating cart total for user: {UserId}", userId);
            var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
            var total = cartItems.Sum(ci => ci.ProductVariant.Price * ci.Quantity);
            return total;
        }
    }
}