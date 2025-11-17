namespace AgricultureBackEnd.DTOs.CouponDTOs
{
    public class CreateCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
