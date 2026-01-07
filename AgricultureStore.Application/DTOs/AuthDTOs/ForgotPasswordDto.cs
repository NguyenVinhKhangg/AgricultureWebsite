using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.AuthDTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}
