using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.CategoryDTOs
{
    public class UpdateCategoryDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string? CategoryName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid parent category ID")]
        public int? ParentCategoryId { get; set; }
    }
}
