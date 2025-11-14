namespace AgricultureBackEnd.DTOs.ProductVariantDTOs
{
    public class UpdateProductVariantDto
    {
        public string? VariantName { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
