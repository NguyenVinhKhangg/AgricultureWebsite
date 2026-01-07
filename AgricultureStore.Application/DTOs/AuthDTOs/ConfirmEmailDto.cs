using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.AuthDTOs
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
    }
}
