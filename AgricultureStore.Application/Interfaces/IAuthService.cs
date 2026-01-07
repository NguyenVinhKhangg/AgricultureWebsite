using AgricultureStore.Application.DTOs.AuthDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task<string> GenerateJwtTokenAsync(int userId, string username, string roleName);
    }
}