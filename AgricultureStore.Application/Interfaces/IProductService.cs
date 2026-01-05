using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.ProductDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Get all products with optional pagination and filtering.
        /// If filterParams is null, returns first page with default page size.
        /// </summary>
        Task<PagedResult<ProductListDto>> GetAllProductsAsync(ProductFilterParams? filterParams = null);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<PagedResult<ProductListDto>> GetProductsByCategoryAsync(int categoryId, PaginationParams? paginationParams = null);
        Task<PagedResult<ProductListDto>> SearchProductsAsync(string searchTerm, PaginationParams? paginationParams = null);
        Task<IEnumerable<ProductListDto>> GetFeaturedProductsAsync(int count);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);
    }
}