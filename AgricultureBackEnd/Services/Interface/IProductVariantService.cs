using AgricultureBackEnd.DTOs.ProductVariantDTOs;

namespace AgricultureBackEnd.Services.Interface
{
    public interface IProductVariantService
    {
        Task<IEnumerable<ProductVariantDto>> GetAllVariantsAsync();
        Task<ProductVariantDto?> GetVariantByIdAsync(int id);
        Task<IEnumerable<ProductVariantDto>> GetByProductIdAsync(int productId);
        Task<IEnumerable<ProductVariantDto>> GetLowStockVariantsAsync(int threshold);
        Task<ProductVariantDto> CreateVariantAsync(CreateProductVariantDto createDto);
        Task<bool> UpdateVariantAsync(int id, UpdateProductVariantDto updateDto);
        Task<bool> DeleteVariantAsync(int id);
        Task<bool> UpdateStockAsync(int variantId, int quantity);
    }
}