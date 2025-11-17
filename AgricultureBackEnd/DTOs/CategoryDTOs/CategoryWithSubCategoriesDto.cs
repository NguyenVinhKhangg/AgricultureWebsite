namespace AgricultureBackEnd.DTOs.CategoryDTOs
{
    public class CategoryWithSubCategoriesDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public List<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
    }
}
