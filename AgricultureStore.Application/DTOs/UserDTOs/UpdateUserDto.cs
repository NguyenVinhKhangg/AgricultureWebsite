namespace AgricultureStore.Application.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? IsActive { get; set; }
    }
}
