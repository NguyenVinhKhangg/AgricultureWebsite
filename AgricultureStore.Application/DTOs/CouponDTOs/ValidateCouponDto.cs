using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.CouponDTOs
{
    public class ValidateCouponDto
    {
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        public string Code { get; set; } = string.Empty;
    }
}
