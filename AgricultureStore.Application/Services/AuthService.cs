using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AgricultureStore.Application.DTOs.AuthDTOs;
using AgricultureStore.Application.Exceptions;
using AgricultureStore.Application.Interfaces;
using AgricultureStore.Application.Settings;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AgricultureStore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork, 
            ILogger<AuthService> logger,
            IOptions<JwtSettings> jwtSettings,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Registration attempt for username: {Username}, email: {Email}", 
                registerDto.UserName, registerDto.Email);

            // Check if username already exists
            if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.UserName))
            {
                _logger.LogWarning("Registration failed - username already exists: {Username}", registerDto.UserName);
                throw new DuplicateException($"Username '{registerDto.UserName}' is already taken");
            }

            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
                throw new DuplicateException($"Email '{registerDto.Email}' is already registered");
            }

            // Generate email confirmation token
            var confirmationToken = GenerateSecureToken();

            // Create new user with default role (User = 2)
            var user = new User
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                RoleId = 2, // Default role: User
                IsActive = true,
                EmailConfirmed = false,
                EmailConfirmationToken = confirmationToken,
                EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email, user.UserName, confirmationToken);

            _logger.LogInformation("User registered successfully - UserId: {UserId}, Username: {Username}", 
                user.UserId, user.UserName);

            return new RegisterResponseDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Message = "Registration successful. Please check your email to confirm your account."
            };
        }

        public async Task<bool> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            _logger.LogInformation("Email confirmation attempt for: {Email}", confirmEmailDto.Email);

            var user = await _unitOfWork.Users.GetByEmailAsync(confirmEmailDto.Email);
            
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed - user not found: {Email}", confirmEmailDto.Email);
                throw new NotFoundException("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogWarning("Email already confirmed for: {Email}", confirmEmailDto.Email);
                throw new BadRequestException("Email is already confirmed");
            }

            if (user.EmailConfirmationToken != confirmEmailDto.Token)
            {
                _logger.LogWarning("Invalid confirmation token for: {Email}", confirmEmailDto.Email);
                throw new BadRequestException("Invalid confirmation token");
            }

            if (user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Confirmation token expired for: {Email}", confirmEmailDto.Email);
                throw new BadRequestException("Confirmation token has expired. Please request a new one.");
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiry = null;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Email confirmed successfully for: {Email}", confirmEmailDto.Email);
            return true;
        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            _logger.LogInformation("Resend confirmation email request for: {Email}", email);

            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            
            if (user == null)
            {
                _logger.LogWarning("Resend confirmation failed - user not found: {Email}", email);
                throw new NotFoundException("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogWarning("Email already confirmed for: {Email}", email);
                throw new BadRequestException("Email is already confirmed");
            }

            // Generate new token
            var newToken = GenerateSecureToken();
            user.EmailConfirmationToken = newToken;
            user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Send new confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email!, user.UserName, newToken);

            _logger.LogInformation("Confirmation email resent to: {Email}", email);
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            _logger.LogInformation("Password reset request for: {Email}", forgotPasswordDto.Email);

            var user = await _unitOfWork.Users.GetByEmailAsync(forgotPasswordDto.Email);
            
            if (user == null)
            {
                // Don't reveal that the user doesn't exist for security reasons
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", forgotPasswordDto.Email);
                return true; // Return true to not reveal if email exists
            }

            // Generate password reset token
            var resetToken = GenerateSecureToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Send password reset email
            await _emailService.SendPasswordResetAsync(user.Email!, user.UserName, resetToken);

            _logger.LogInformation("Password reset email sent to: {Email}", forgotPasswordDto.Email);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Password reset attempt for: {Email}", resetPasswordDto.Email);

            var user = await _unitOfWork.Users.GetByEmailAsync(resetPasswordDto.Email);
            
            if (user == null)
            {
                _logger.LogWarning("Password reset failed - user not found: {Email}", resetPasswordDto.Email);
                throw new NotFoundException("User not found");
            }

            if (user.PasswordResetToken != resetPasswordDto.Token)
            {
                _logger.LogWarning("Invalid reset token for: {Email}", resetPasswordDto.Email);
                throw new BadRequestException("Invalid reset token");
            }

            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Reset token expired for: {Email}", resetPasswordDto.Email);
                throw new BadRequestException("Reset token has expired. Please request a new one.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password reset successfully for: {Email}", resetPasswordDto.Email);
            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for username: {Username}", loginDto.UserName);

            var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.UserName);
            
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Username}", loginDto.UserName);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed - inactive user: {Username}", loginDto.UserName);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed - invalid password for user: {Username}", loginDto.UserName);
                return null;
            }

            var roleName = user.Role?.RoleName ?? "User";
            var token = await GenerateJwtTokenAsync(user.UserId, user.UserName, roleName);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            _logger.LogInformation("Login successful - UserId: {UserId}, Username: {Username}, Role: {Role}",
                user.UserId, user.UserName, roleName);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = expiration,
                UserId = user.UserId,
                UserName = user.UserName,
                RoleName = roleName
            };
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

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}