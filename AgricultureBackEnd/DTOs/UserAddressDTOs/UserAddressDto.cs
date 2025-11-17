namespace AgricultureBackEnd.DTOs.UserAddressDTOs
{
    public class UserAddressDto
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string? AddressLine { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
