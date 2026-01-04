using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.ProductVariantDTOs
{
    public class UpdateProductVariantDto
    {
        [StringLength(100, ErrorMessage = "Variant name cannot exceed 100 characters")]
        public string? VariantName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int? StockQuantity { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}
