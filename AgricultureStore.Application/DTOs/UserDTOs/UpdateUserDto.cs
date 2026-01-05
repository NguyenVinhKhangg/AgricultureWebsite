using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Phone number must be 10-11 digits")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [Range(0, 1, ErrorMessage = "IsActive must be 0 or 1")]
        public int? IsActive { get; set; }
    }
}
