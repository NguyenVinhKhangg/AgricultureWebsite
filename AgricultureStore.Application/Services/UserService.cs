using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.UserDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace AgricultureStore.Application.Services
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

        public async Task<PagedResult<UserDto>> GetAllUsersAsync(UserFilterParams? filterParams = null)
        {
            filterParams ??= new UserFilterParams();
            
            _logger.LogDebug("Getting users - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                filterParams.PageNumber, filterParams.PageSize, filterParams.SearchTerm);

            var (users, totalCount) = await _unitOfWork.Users.GetUsersPagedAsync(
                filterParams.PageNumber,
                filterParams.PageSize,
                filterParams.SearchTerm,
                filterParams.Role,
                filterParams.IsActive,
                filterParams.SortBy ?? "CreatedAt",
                filterParams.SortDescending);

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return new PagedResult<UserDto>(userDtos, totalCount, filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            _logger.LogDebug("Getting user with ID: {UserId}", id);
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            _logger.LogDebug("Getting user by username: {Username}", username);
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);

            if (user == null)
            {
                _logger.LogWarning("User with username {Username} not found", username);
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            _logger.LogDebug("Getting user by email: {Email}", email);
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
        {
            _logger.LogInformation("Creating new user: {Username}", createDto.UserName);

            if (await _unitOfWork.Users.UsernameExistsAsync(createDto.UserName))
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (!string.IsNullOrEmpty(createDto.Email) && await _unitOfWork.Users.EmailExistsAsync(createDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            var user = _mapper.Map<User>(createDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User created - UserId: {UserId}, Username: {Username}", user.UserId, user.UserName);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto)
        {
            _logger.LogDebug("Updating user with ID: {UserId}", id);

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                return false;
            }

            _mapper.Map(updateDto, user);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User updated - UserId: {UserId}", id);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            _logger.LogDebug("Soft deleting user with ID: {UserId}", id);

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return false;
            }

            user.IsActive = false;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User soft-deleted - UserId: {UserId}", id);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            _logger.LogDebug("Changing password for user: {UserId}", userId);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for password change", userId);
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                _logger.LogWarning("Invalid current password for user: {UserId}", userId);
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password changed - UserId: {UserId}", userId);
            return true;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            _logger.LogDebug("Checking if username exists: {Username}", username);
            return await _unitOfWork.Users.UsernameExistsAsync(username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            _logger.LogDebug("Checking if email exists: {Email}", email);
            return await _unitOfWork.Users.EmailExistsAsync(email);
        }
    }
}