using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IProductVariantRepository : IRepository<ProductVariant>
    {
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId);
        Task<ProductVariant?> GetWithProductAsync(int variantId);
        Task<bool> UpdateStockAsync(int variantId, int quantity);
        Task<IEnumerable<ProductVariant>> GetLowStockVariantsAsync(int threshold);
    }
}
