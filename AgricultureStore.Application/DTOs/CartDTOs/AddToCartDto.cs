namespace AgricultureStore.Application.DTOs.CartDTOs
{
    public class AddToCartDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; } = 1;

    }
}
