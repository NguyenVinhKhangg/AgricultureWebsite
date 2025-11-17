using AgricultureBackEnd.Models;

namespace AgricultureBackEnd.Repositories.Interface
{
    public interface IProductVariantRepository : IRepository<ProductVariant>
    {
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId);
        Task<ProductVariant?> GetWithProductAsync(int variantId);
        Task<bool> UpdateStockAsync(int variantId, int quantity);
        Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int threshold);
    }
}