using AgricultureBackEnd.Models;

namespace AgricultureBackEnd.Repositories.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<Category?> GetWithSubCategoriesAsync(int categoryId);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId);
        Task<bool> HasProductsAsync(int categoryId);
    }
}