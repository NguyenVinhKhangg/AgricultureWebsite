using AgricultureStore.Application.DTOs.UserAddressDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;

        public UserAddressController(IUserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserAddressDto>>> GetAddressesByUserId(int userId)
        {
            var addresses = await _userAddressService.GetAddressByIdAsync(userId);
            return Ok(addresses);
        }

        [HttpGet("user/{userId}/default")]
        public async Task<ActionResult<UserAddressDto>> GetUserDefaultAddress(int userId)
        {
            var address = await _userAddressService.GetDefaultAddressAsync(userId);
            if (address == null)
            {
                return NotFound($"Default address for user with ID {userId} not found");
            }
            return Ok(address);
        }
        
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<UserAddressDto>> CreateUserAddress(int userId, [FromBody] CreateUserAddressDto createDto)
        {
            var newAddress = await _userAddressService.CreateAddressAsync(userId, createDto);
            return CreatedAtAction(nameof(GetAddressesByUserId), new { userId = userId }, newAddress);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAddress(int id, [FromBody] UpdateUserAddressDto updateDto)
        {
            var result = await _userAddressService.UpdateAddressAsync(id, updateDto);
            if (!result)
            {
                return NotFound($"Address with ID {id} not found");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            var result = await _userAddressService.DeleteAddressAsync(id);
            if (!result)
            {
                return NotFound($"Address with ID {id} not found");
            }
            return NoContent();
        } 

        [HttpPut("user/{userId}/default/{addressId}")] 
        public async Task<IActionResult> SetDefaultUserAddress(int userId, int addressId)
        {
            var result = await _userAddressService.SetDefaultAddressAsync(userId, addressId);
            if (!result)
            {
                return NotFound($"Address with ID {addressId} for user with ID {userId} not found");
            }
            return NoContent();
        }
    }
}