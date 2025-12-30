using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);
        Task<Product?> GetWithVariantsAsync(int productId);
        Task<Product?> GetWithDetailsAsync(int productId);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count);
    }
}
