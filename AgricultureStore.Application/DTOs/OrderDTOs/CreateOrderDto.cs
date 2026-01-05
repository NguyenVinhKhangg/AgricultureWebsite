using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.OrderDTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Shipping address must be between 10 and 500 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
        public string? PaymentMethod { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }

        [StringLength(50, ErrorMessage = "Coupon code cannot exceed 50 characters")]
        public string? CouponCode { get; set; }
    }
}
