using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ILogger<CartController> _logger;
        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetCartByUserId(int userId)
        {
            try
            {
                var cart =  _cartService.GetCartItemsAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning($"Cart not found for user with ID {userId}");
                    return NotFound();
                }
            return Ok(cart);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("user/{userId}/add")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] AddToCartDto addToCartDto)
        {
            try
            {
                var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto);
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding item to cart for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("user/{userId}/update")]
        public async Task<IActionResult> UpdateCartItem(int userId, [FromBody] UpdateCartItem updateCartItemDto)
        {
            try
            {
                var updatedCartItem = await _cartService.UpdateCartItemAsync(userId, updateCartItemDto.VariantId, updateCartItemDto.Quantity);
                if (!updatedCartItem)
                {
                    return NotFound($"Cart item not found for user with ID {userId}");
                }
                return Ok(updatedCartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart item for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("user/{userId}/remove/{variantId}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int variantId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(userId, variantId);
                if (!result)
                {
                    return NotFound($"Cart item not found for user with ID {userId}");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item from cart for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}/total")]
        public async Task<IActionResult> GetCartTotalAsync(int userId)
        {
            try
            {
                var total = await _cartService.GetCartTotalAsync(userId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating cart total for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}/count")]
        public async Task<IActionResult> GetCartItemCountAsync(int userId)
        {
            try
            {
                var itemCount = await _cartService.GetCartItemCountAsync(userId);
                return Ok(itemCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cart item count for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("user/{userId}/clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                var result = await _cartService.ClearCartAsync(userId);
                if (!result)
                {
                    return NotFound($"No items to clear for user with ID {userId}");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing cart for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }   
    }
}