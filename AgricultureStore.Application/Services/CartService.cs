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
            try
            {
                _logger.LogInformation("Getting cart items for user: {UserId}", userId);
                var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
                _logger.LogInformation("Retrieved {Count} cart items for user {UserId}", cartItems.Count(), userId);
                return _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            try
            {
                _logger.LogInformation("Adding item to cart - User: {UserId}, Variant: {VariantId}, Quantity: {Quantity}",
                    userId, addToCartDto.VariantId, addToCartDto.Quantity);

                var existingItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, addToCartDto.VariantId);

                if (existingItem != null)
                {
                    _logger.LogInformation("Item already in cart, updating quantity from {OldQuantity} to {NewQuantity}",
                        existingItem.Quantity, existingItem.Quantity + addToCartDto.Quantity);

                    existingItem.Quantity += addToCartDto.Quantity;
                    await _unitOfWork.CartItems.UpdateAsync(existingItem);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Cart item updated successfully");
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

                _logger.LogInformation("New item added to cart successfully");

                var addedItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, addToCartDto.VariantId);
                return _mapper.Map<CartItemDto>(addedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart - User: {UserId}, Variant: {VariantId}",
                    userId, addToCartDto.VariantId);
                throw;
            }
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int variantId, int quantity)
        {
            try
            {
                _logger.LogInformation("Updating cart item - User: {UserId}, Variant: {VariantId}, New Quantity: {Quantity}",
                    userId, variantId, quantity);

                var cartItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, variantId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item not found - User: {UserId}, Variant: {VariantId}", userId, variantId);
                    return false;
                }

                if (quantity <= 0)
                {
                    _logger.LogInformation("Quantity <= 0, removing item from cart");
                    return await RemoveFromCartAsync(userId, variantId);
                }

                cartItem.Quantity = quantity;
                await _unitOfWork.CartItems.UpdateAsync(cartItem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cart item updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item - User: {UserId}, Variant: {VariantId}", userId, variantId);
                throw;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int variantId)
        {
            try
            {
                _logger.LogInformation("Removing item from cart - User: {UserId}, Variant: {VariantId}", userId, variantId);
                var result = await _unitOfWork.CartItems.RemoveByUserAndVariantAsync(userId, variantId);

                if (result)
                {
                    _logger.LogInformation("Item removed from cart successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to remove item from cart - not found");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart - User: {UserId}, Variant: {VariantId}", userId, variantId);
                throw;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Clearing cart for user: {UserId}", userId);
                var result = await _unitOfWork.CartItems.ClearCartAsync(userId);

                if (result)
                {
                    _logger.LogInformation("Cart cleared successfully for user: {UserId}", userId);
                }
                else
                {
                    _logger.LogWarning("No items to clear for user: {UserId}", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            try
            {
                _logger.LogDebug("Getting cart item count for user: {UserId}", userId);
                return await _unitOfWork.CartItems.GetCartItemCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Calculating cart total for user: {UserId}", userId);
                var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
                var total = cartItems.Sum(ci => ci.ProductVariant.Price * ci.Quantity);
                _logger.LogInformation("Cart total for user {UserId}: {Total:C}", userId, total);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cart total for user: {UserId}", userId);
                throw;
            }
        }
    }
}