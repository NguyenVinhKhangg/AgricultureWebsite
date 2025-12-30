namespace AgricultureStore.Application.DTOs.UserDTOs
{
    public class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; } 
        public string? Phone { get; set; }
        public int RoleId { get; set; }

    }
}
