using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.UserDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterParams? filterParams = null);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createDto);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
