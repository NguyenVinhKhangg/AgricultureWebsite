using AgricultureStore.Application.DTOs.UserAddressDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
namespace AgricultureStore.Application.Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAddressService> _logger;

        public UserAddressService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserAddressService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserAddressDto>> GetUserAddressesAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Getting addresses for user: {UserId}", userId);
                var addresses = await _unitOfWork.UserAddresses.GetByUserIdAsync(userId);
                _logger.LogInformation("Retrieved {Count} addresses for user {UserId}", addresses.Count(), userId);
                return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserAddressDto?> GetAddressByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting address with ID: {AddressId}", id);
                var address = await _unitOfWork.UserAddresses.GetByIdAsync(id);

                if (address == null)
                {
                    _logger.LogWarning("Address with ID {AddressId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved address: {AddressId}", id);
                return _mapper.Map<UserAddressDto>(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address with ID: {AddressId}", id);
                throw;
            }
        }

        public async Task<UserAddressDto?> GetDefaultAddressAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Getting default address for user: {UserId}", userId);
                var address = await _unitOfWork.UserAddresses.GetDefaultAddressAsync(userId);

                if (address == null)
                {
                    _logger.LogWarning("No default address found for user: {UserId}", userId);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved default address for user: {UserId}", userId);
                return _mapper.Map<UserAddressDto>(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default address for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserAddressDto> CreateAddressAsync(int userId, CreateUserAddressDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new address for user: {UserId}", userId);

                var address = _mapper.Map<UserAddress>(createDto);
                address.UserId = userId;

                await _unitOfWork.UserAddresses.AddAsync(address);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Address created successfully with ID: {AddressId} for user: {UserId}",
                    address.AddressId, userId);

                if (createDto.IsDefault)
                {
                    _logger.LogInformation("Setting address {AddressId} as default for user {UserId}", address.AddressId, userId);
                    await _unitOfWork.UserAddresses.SetDefaultAddressAsync(userId, address.AddressId);
                    await _unitOfWork.SaveChangesAsync();
                }

                return _mapper.Map<UserAddressDto>(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateAddressAsync(int id, UpdateUserAddressDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating address with ID: {AddressId}", id);

                var address = await _unitOfWork.UserAddresses.GetByIdAsync(id);
                if (address == null)
                {
                    _logger.LogWarning("Address with ID {AddressId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, address);
                await _unitOfWork.UserAddresses.UpdateAsync(address);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Address updated successfully: {AddressId}", id);

                if (updateDto.IsDefault == true)
                {
                    _logger.LogInformation("Setting address {AddressId} as default", id);
                    await _unitOfWork.UserAddresses.SetDefaultAddressAsync(address.UserId, id);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address with ID: {AddressId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting address with ID: {AddressId}", id);

                await _unitOfWork.UserAddresses.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Address deleted successfully: {AddressId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address with ID: {AddressId}", id);
                throw;
            }
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            try
            {
                _logger.LogInformation("Setting default address {AddressId} for user {UserId}", addressId, userId);

                var result = await _unitOfWork.UserAddresses.SetDefaultAddressAsync(userId, addressId);

                if (result)
                {
                    _logger.LogInformation("Default address set successfully - User: {UserId}, Address: {AddressId}", userId, addressId);
                }
                else
                {
                    _logger.LogWarning("Failed to set default address - User: {UserId}, Address: {AddressId}", userId, addressId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default address {AddressId} for user {UserId}", addressId, userId);
                throw;
            }
        }
    }
}