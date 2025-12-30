namespace AgricultureStore.Domain.Entities
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string? Code { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
