namespace AgricultureBackEnd.DTOs.CartDTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? VariantName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
