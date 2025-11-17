namespace AgricultureBackEnd.DTOs.OrderDTOs
{
    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public string? CouponCode { get; set; }
    }
}
