namespace AgricultureBackEnd.DTOs.CouponDTOs
{
    public class UpdateCouponDto
    {
        public string? Code { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
