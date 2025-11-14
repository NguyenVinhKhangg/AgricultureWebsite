namespace AgricultureBackEnd.DTOs.ProductVariantDTOs
{
    public class CreateProductVariantDto
    {
        public int ProductId { get; set; }
        public string? VariantName { get; set; } 
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
