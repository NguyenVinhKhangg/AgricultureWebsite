using AgricultureBackEnd.DTOs.UserDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;
using BCrypt.Net;

namespace AgricultureBackEnd.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var users = await _unitOfWork.Users.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} users", users.Count());
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting user with ID: {UserId}", id);
                var user = await _unitOfWork.Users.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user: {Username}", user.UserName);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            try
            {
                _logger.LogInformation("Getting user by username: {Username}", username);
                var user = await _unitOfWork.Users.GetByUsernameAsync(username);

                if (user == null)
                {
                    _logger.LogWarning("User with username {Username} not found", username);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user: {Username}", username);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Getting user by email: {Email}", email);
                var user = await _unitOfWork.Users.GetByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found", email);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user with email: {Email}", email);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new user: {Username}", createDto.UserName);

                if (await _unitOfWork.Users.UsernameExistsAsync(createDto.UserName))
                {
                    _logger.LogWarning("Username {Username} already exists", createDto.UserName);
                    throw new InvalidOperationException("Username already exists");
                }

                if (!string.IsNullOrEmpty(createDto.Email) && await _unitOfWork.Users.EmailExistsAsync(createDto.Email))
                {
                    _logger.LogWarning("Email {Email} already exists", createDto.Email);
                    throw new InvalidOperationException("Email already exists");
                }

                var user = _mapper.Map<User>(createDto);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {Username} with ID: {UserId}", user.UserName, user.UserId);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", createDto.UserName);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating user with ID: {UserId}", id);

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, user);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User updated successfully: {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Soft deleting user with ID: {UserId}", id);

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                    return false;
                }

                user.IsActive = false;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User soft-deleted successfully: {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                _logger.LogInformation("Changing password for user: {UserId}", userId);

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for password change", userId);
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid old password for user: {UserId}", userId);
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                _logger.LogDebug("Checking if username exists: {Username}", username);
                return await _unitOfWork.Users.UsernameExistsAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username existence: {Username}", username);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                _logger.LogDebug("Checking if email exists: {Email}", email);
                return await _unitOfWork.Users.EmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}", email);
                throw;
            }
        }
    }
}