using AgricultureStore.Application.DTOs.Common;

namespace AgricultureStore.Application.DTOs.UserDTOs
{
    public class UserFilterParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "CreatedAt"; // UserName, Email, CreatedAt
        public bool SortDescending { get; set; } = true;
    }
}
