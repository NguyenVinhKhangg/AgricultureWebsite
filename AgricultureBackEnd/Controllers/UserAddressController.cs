using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgricultureBackEnd.Controllers
{
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;
        private readonly ILogger<UserAddressController> _logger;

        public UserAddressController(IUserAddressService userAddressService, ILogger<UserAddressController> logger)
        {
            _userAddressService = userAddressService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserAddressDto>>> GetAddressesByUserId(int userId)
        {
            try
            {
                var addresses = await _userAddressService.GetAddressesByUserIdAsync(userId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting addresses for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}/default")]
        public async Task<IActionResult<UserAddressDto>> GetUserDefaultAddress(int userId){
            try{
                var address = await _userAddressService.GetUserDefaultAddressAsync(userId);
                if(address == null){
                    return NotFound($"Default address for user with ID {userId} not found");
                }
                return Ok(address);

            }catch(Exception ex){
                _logger.LogError(ex, "Error occurred while getting default address for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<UserAddressDto>> CreateUserAddress(int userId, [FromBody] CreateUserAddressDto createDto)
        {
            try
            {
                var newAddress = await _userAddressService.CreateUserAddressAsync(userId, createDto);
                return CreatedAtAction(nameof(GetAddressesByUserId), new { userId = userId }, newAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating address for user with ID: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAddress(int id, [FromBody] UpdateUserAddressDto updateDto)
        {
            try
            {
                var result = await _userAddressService.UpdateUserAddressAsync(id, updateDto);
                if (!result)
                {
                    return NotFound($"Address with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating address with ID: {AddressId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            try
            {
                var result = await _userAddressService.DeleteUserAddressAsync(id);
                if (!result)
                {
                    return NotFound($"Address with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting address with ID: {AddressId}", id);
                return StatusCode(500, "Internal server error");
            }
        } 

        [HttpPut("user/{userId}/default/{addressId}")] 
        public async Task<IActionResult> SetDefaultUserAddress(int userId, int addressId)
        {
            try
            {
                var result = await _userAddressService.SetDefaultUserAddressAsync(userId, addressId);
                if (!result)
                {
                    return NotFound($"Address with ID {addressId} for user with ID {userId} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting default address with ID: {AddressId} for user with ID: {UserId}", addressId, userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}