using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.AuthDTOs;
using AgricultureStore.Application.Interfaces;
using AgricultureStore.Application.Settings;
using AgricultureStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgricultureStore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUnitOfWork unitOfWork, 
            ILogger<AuthService> logger,
            IOptions<JwtSettings> jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", loginDto.UserName);

                var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.UserName);
                
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", loginDto.UserName);
                    return null;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Inactive user attempted login: {Username}", loginDto.UserName);
                    return null;
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for user: {Username}", loginDto.UserName);
                    return null;
                }

                var roleName = user.Role?.RoleName ?? "User";
                var token = await GenerateJwtTokenAsync(user.UserId, user.UserName, roleName);
                var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

                _logger.LogInformation("Login successful for user: {Username}", loginDto.UserName);

                return new LoginResponseDto
                {
                    Token = token,
                    Expiration = expiration,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    RoleName = roleName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", loginDto.UserName);
                throw;
            }
        }

        public async Task<string> GenerateJwtTokenAsync(int userId, string username, string roleName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
        
    }
}