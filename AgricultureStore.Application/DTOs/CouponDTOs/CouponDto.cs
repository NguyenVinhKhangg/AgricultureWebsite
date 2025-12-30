namespace AgricultureStore.Application.DTOs.CouponDTOs
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string? Code { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsValid { get; set; }
        public bool IsActive { get; set; }
    }
}
