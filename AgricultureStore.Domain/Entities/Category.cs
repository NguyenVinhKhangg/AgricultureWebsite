namespace AgricultureStore.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }
}
