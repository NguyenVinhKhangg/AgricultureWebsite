using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.CouponDTOs
{
    public class CalculateDiscountDto
    {
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Order amount must be greater than 0")]
        public decimal OrderAmount { get; set; }
    }
}