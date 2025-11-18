using AgricultureBackEnd.DTOs.CartDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(int userId)
        {
            var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
        }

        public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            var existingItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, addToCartDto.VariantId);

            if (existingItem != null)
            {
                existingItem.Quantity += addToCartDto.Quantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
                await _unitOfWork.SaveChangesAsync();
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

            var addedItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, addToCartDto.VariantId);
            return _mapper.Map<CartItemDto>(addedItem);
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int variantId, int quantity)
        {
            var cartItem = await _unitOfWork.CartItems.GetCartItemAsync(userId, variantId);
            if (cartItem == null) return false;

            if (quantity <= 0)
                return await RemoveFromCartAsync(userId, variantId);

            cartItem.Quantity = quantity;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int variantId)
        {
            return await _unitOfWork.CartItems.RemoveByUserAndVariantAsync(userId, variantId);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            return await _unitOfWork.CartItems.ClearCartAsync(userId);
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            return await _unitOfWork.CartItems.GetCartItemCountAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var cartItems = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
            return cartItems.Sum(ci => ci.ProductVariant.Price * ci.Quantity);
        }
    }
}