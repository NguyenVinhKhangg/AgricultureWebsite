using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.ProductDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductListDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductListDto>> GetFeaturedProductsAsync(int count);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);

        // Paginated methods
        Task<PagedResult<ProductListDto>> GetProductsPagedAsync(ProductFilterParams filterParams);
        Task<PagedResult<ProductListDto>> GetProductsByCategoryPagedAsync(int categoryId, PaginationParams paginationParams);
    }
}