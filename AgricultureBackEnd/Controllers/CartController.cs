using AgricultureStore.Application.DTOs.CartDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var cart = await _cartService.GetCartItemsAsync(userId);
            return Ok(cart);
        }

        [HttpPost("user/{userId}/add")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] AddToCartDto addToCartDto)
        {
            var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto);
            return Ok(cartItem);
        }

        [HttpPut("user/{userId}/update")]
        public async Task<IActionResult> UpdateCartItem(int userId, [FromBody] UpdateCartItem updateCartItemDto)
        {
            var updatedCartItem = await _cartService.UpdateCartItemAsync(userId, updateCartItemDto.VariantId, updateCartItemDto.Quantity);
            if (!updatedCartItem)
            {
                return NotFound($"Cart item not found for user with ID {userId}");
            }
            return Ok(updatedCartItem);
        }

        [HttpDelete("user/{userId}/remove/{variantId}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int variantId)
        {
            var result = await _cartService.RemoveFromCartAsync(userId, variantId);
            if (!result)
            {
                return NotFound($"Cart item not found for user with ID {userId}");
            }
            return NoContent();
        }

        [HttpGet("user/{userId}/total")]
        public async Task<IActionResult> GetCartTotalAsync(int userId)
        {
            var total = await _cartService.GetCartTotalAsync(userId);
            return Ok(total);
        }

        [HttpGet("user/{userId}/count")]
        public async Task<IActionResult> GetCartItemCountAsync(int userId)
        {
            var itemCount = await _cartService.GetCartItemCountAsync(userId);
            return Ok(itemCount);
        }

        [HttpDelete("user/{userId}/clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var result = await _cartService.ClearCartAsync(userId);
            if (!result)
            {
                return NotFound($"No items to clear for user with ID {userId}");
            }
            return NoContent();
        }   
    }
}