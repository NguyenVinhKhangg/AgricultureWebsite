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
            _logger.LogDebug("Getting all active products");
            var products = await _unitOfWork.Products.GetActiveProductsAsync();
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            _logger.LogDebug("Getting product with ID: {ProductId}", id);
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(int categoryId)
        {
            _logger.LogDebug("Getting products by category ID: {CategoryId}", categoryId);
            var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<IEnumerable<ProductListDto>> SearchProductsAsync(string searchTerm)
        {
            _logger.LogDebug("Searching products with term: {SearchTerm}", searchTerm);
            var products = await _unitOfWork.Products.SearchByNameAsync(searchTerm);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<IEnumerable<ProductListDto>> GetFeaturedProductsAsync(int count)
        {
            _logger.LogDebug("Getting {Count} featured products", count);
            var products = await _unitOfWork.Products.GetFeaturedProductsAsync(count);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            _logger.LogInformation("Creating new product: {ProductName}", createDto.ProductName);

            var product = _mapper.Map<Product>(createDto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product created - ProductId: {ProductId}, Name: {ProductName}",
                product.ProductId, product.ProductName);

            var createdProduct = await _unitOfWork.Products.GetWithDetailsAsync(product.ProductId);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            _logger.LogDebug("Updating product with ID: {ProductId}", id);

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return false;
            }

            _mapper.Map(updateDto, product);
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product updated - ProductId: {ProductId}", id);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogDebug("Soft deleting product with ID: {ProductId}", id);

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                return false;
            }

            product.IsActive = false;
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product soft-deleted - ProductId: {ProductId}", id);
            return true;
        }
    }
}