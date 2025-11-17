namespace AgricultureBackEnd.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string? ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public string Status { get; set; } = "Pending";
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public int? CouponId { get; set; }
        // Navigation properties
        public User User { get; set; } = null!;
        public Coupon? Coupon { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
