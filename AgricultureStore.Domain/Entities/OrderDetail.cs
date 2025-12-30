namespace AgricultureStore.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int VariantId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
