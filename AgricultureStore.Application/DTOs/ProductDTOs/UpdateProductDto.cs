using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.ProductDTOs
{
    public class UpdateProductDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid category ID")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid image URL format")]
        public string? ImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters")]
        public string? SupplierName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
