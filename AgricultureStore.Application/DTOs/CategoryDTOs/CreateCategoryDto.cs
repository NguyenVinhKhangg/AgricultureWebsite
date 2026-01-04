using System.ComponentModel.DataAnnotations;

namespace AgricultureStore.Application.DTOs.CategoryDTOs
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid parent category ID")]
        public int? ParentCategoryId { get; set; }
    }
}
