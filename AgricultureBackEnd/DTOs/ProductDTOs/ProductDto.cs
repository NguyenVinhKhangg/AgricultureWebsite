using AgricultureBackEnd.DTOs.ProductVariantDTOs;
using AgricultureBackEnd.Models;

namespace AgricultureBackEnd.DTOs.ProductDTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? SupplierName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductVariantDto> Variants { get; set; }

    }
}
