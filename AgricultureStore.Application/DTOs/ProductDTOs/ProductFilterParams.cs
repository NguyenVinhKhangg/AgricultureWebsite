using AgricultureStore.Application.DTOs.Common;

namespace AgricultureStore.Application.DTOs.ProductDTOs
{
    public class ProductFilterParams : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = "CreatedAt"; // Name, Price, CreatedAt
        public bool SortDescending { get; set; } = true;
    }
}
