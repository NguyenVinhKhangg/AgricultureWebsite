using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.ProductVariantDTOs
{
    public class CreateProductVariantDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid product ID")]
        public int ProductId { get; set; }

        [StringLength(100, ErrorMessage = "Variant name cannot exceed 100 characters")]
        public string? VariantName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
