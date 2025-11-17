namespace AgricultureBackEnd.Models
{
    public class UserAddress
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string? AddressLine { get; set; }
        public bool IsDefault { get; set; } = false;

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
