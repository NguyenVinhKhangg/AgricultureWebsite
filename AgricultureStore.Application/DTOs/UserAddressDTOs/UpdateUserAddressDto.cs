using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.UserAddressDTOs
{
    public class UpdateUserAddressDto
    {
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 500 characters")]
        public string? AddressLine { get; set; }

        public bool IsDefault { get; set; }
    }
}
