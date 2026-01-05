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

        // Paginated methods
        Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "CreatedAt",
            bool sortDescending = true);

        Task<(IEnumerable<Product> Items, int TotalCount)> GetByCategoryPagedAsync(
            int categoryId,
            int pageNumber,
            int pageSize);
    }
}
