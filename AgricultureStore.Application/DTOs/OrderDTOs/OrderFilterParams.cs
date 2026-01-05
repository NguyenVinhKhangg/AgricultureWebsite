using AgricultureStore.Application.DTOs.Common;

namespace AgricultureStore.Application.DTOs.OrderDTOs
{
    public class OrderFilterParams : PaginationParams
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? UserId { get; set; }
        public string? SortBy { get; set; } = "OrderDate"; // OrderDate, TotalAmount
        public bool SortDescending { get; set; } = true;
    }
}
