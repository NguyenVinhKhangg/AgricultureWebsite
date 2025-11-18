using AgricultureBackEnd.DTOs.ProductDTOs;
using AgricultureBackEnd.Models;
using AgricultureBackEnd.Repositories.Interface;
using AgricultureBackEnd.Services.Interface;
using AutoMapper;

namespace AgricultureBackEnd.Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetActiveProductsAsync();
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<IEnumerable<ProductListDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _unitOfWork.Products.SearchByNameAsync(searchTerm);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<IEnumerable<ProductListDto>> GetFeaturedProductsAsync(int count)
        {
            var products = await _unitOfWork.Products.GetFeaturedProductsAsync(count);
            return _mapper.Map<IEnumerable<ProductListDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            var product = _mapper.Map<Product>(createDto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var createdProduct = await _unitOfWork.Products.GetWithDetailsAsync(product.ProductId);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            _mapper.Map(updateDto, product);
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}