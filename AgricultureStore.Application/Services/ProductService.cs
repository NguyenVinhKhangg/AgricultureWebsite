using AgricultureStore.Application.DTOs.ProductDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;

using Microsoft.Extensions.Logging;
namespace AgricultureStore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all active products");
                var products = await _unitOfWork.Products.GetActiveProductsAsync();
                _logger.LogInformation("Retrieved {Count} active products", products.Count());
                return _mapper.Map<IEnumerable<ProductListDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId}", id);
                var product = await _unitOfWork.Products.GetWithDetailsAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved product: {ProductName} (ID: {ProductId})", product.ProductName, id);
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation("Getting products by category ID: {CategoryId}", categoryId);
                var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
                _logger.LogInformation("Retrieved {Count} products for category {CategoryId}", products.Count(), categoryId);
                return _mapper.Map<IEnumerable<ProductListDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category ID: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductListDto>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
                var products = await _unitOfWork.Products.SearchByNameAsync(searchTerm);
                _logger.LogInformation("Found {Count} products matching: {SearchTerm}", products.Count(), searchTerm);
                return _mapper.Map<IEnumerable<ProductListDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<ProductListDto>> GetFeaturedProductsAsync(int count)
        {
            try
            {
                _logger.LogInformation("Getting {Count} featured products", count);
                var products = await _unitOfWork.Products.GetFeaturedProductsAsync(count);
                _logger.LogInformation("Retrieved {Count} featured products", products.Count());
                return _mapper.Map<IEnumerable<ProductListDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured products");
                throw;
            }
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", createDto.ProductName);

                var product = _mapper.Map<Product>(createDto);
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product created successfully: {ProductName} with ID: {ProductId}",
                    product.ProductName, product.ProductId);

                var createdProduct = await _unitOfWork.Products.GetWithDetailsAsync(product.ProductId);
                return _mapper.Map<ProductDto>(createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", createDto.ProductName);
                throw;
            }
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);

                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, product);
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product updated successfully: {ProductId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Soft deleting product with ID: {ProductId}", id);

                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                    return false;
                }

                product.IsActive = false;
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product soft-deleted successfully: {ProductId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                throw;
            }
        }
    }
}