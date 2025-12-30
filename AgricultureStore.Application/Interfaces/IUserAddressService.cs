using AgricultureStore.Application.DTOs.UserAddressDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IUserAddressService
    {
        Task<IEnumerable<UserAddressDto>> GetUserAddressesAsync(int userId);
        Task<UserAddressDto?> GetAddressByIdAsync(int id);
        Task<UserAddressDto?> GetDefaultAddressAsync(int userId);
        Task<UserAddressDto> CreateAddressAsync(int userId, CreateUserAddressDto createDto);
        Task<bool> UpdateAddressAsync(int id, UpdateUserAddressDto updateDto);
        Task<bool> DeleteAddressAsync(int id);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }
}