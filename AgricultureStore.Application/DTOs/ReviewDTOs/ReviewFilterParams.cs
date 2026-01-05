using AgricultureStore.Application.DTOs.Common;

namespace AgricultureStore.Application.DTOs.ReviewDTOs
{
    public class ReviewFilterParams : PaginationParams
    {
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public string? SortBy { get; set; } = "CreatedAt"; // CreatedAt, Rating
        public bool SortDescending { get; set; } = true;
    }
}
