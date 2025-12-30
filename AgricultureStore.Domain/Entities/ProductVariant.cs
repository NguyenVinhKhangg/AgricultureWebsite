namespace AgricultureStore.Domain.Entities
{
    public class ProductVariant
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string? VariantName { get; set; } 
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Product? Product { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
