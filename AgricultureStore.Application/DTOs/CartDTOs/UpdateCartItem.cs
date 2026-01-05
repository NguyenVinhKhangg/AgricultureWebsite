using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.CartDTOs
{
    public class UpdateCartItem
    {
        [Required(ErrorMessage = "Variant ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid variant ID")]
        public int VariantId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }
    }
}
