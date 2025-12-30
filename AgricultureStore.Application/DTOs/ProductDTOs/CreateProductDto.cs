namespace AgricultureStore.Application.DTOs.ProductDTOs
{
    public class CreateProductDto
    {
        public int? CategoryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? SupplierName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
