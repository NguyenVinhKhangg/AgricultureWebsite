using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.OrderDTOs
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression(@"^(Pending|Confirmed|Processing|Shipped|Delivered|Cancelled)$",
            ErrorMessage = "Status must be one of: Pending, Confirmed, Processing, Shipped, Delivered, Cancelled")]
        public string Status { get; set; } = string.Empty;
    }
}
