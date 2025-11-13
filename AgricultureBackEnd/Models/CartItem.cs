namespace AgricultureBackEnd.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int VariantId { get; set; }
        public int Quantity { get; set; } = 1 ;

        // Navigation properties
        public User? User { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
