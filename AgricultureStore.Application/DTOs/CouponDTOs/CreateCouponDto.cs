using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.CouponDTOs
{
    public class CreateCouponDto
    {
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Coupon code can only contain uppercase letters, numbers, underscores and hyphens")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Discount value is required")]
        [Range(0.01, 100000, ErrorMessage = "Discount value must be between 0.01 and 100000")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
