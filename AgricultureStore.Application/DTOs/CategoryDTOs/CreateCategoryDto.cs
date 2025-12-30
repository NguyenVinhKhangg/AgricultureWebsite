namespace AgricultureStore.Application.DTOs.CategoryDTOs
{
    public class CreateCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
    }
}
