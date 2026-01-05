using AgricultureStore.Application.DTOs.CategoryDTOs;
using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AgricultureStore.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            _logger.LogDebug("Getting all categories");
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            _logger.LogDebug("Getting category with ID: {CategoryId}", id);
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
                return null;
            }

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
        {
            _logger.LogDebug("Getting root categories");
            var categories = await _unitOfWork.Categories.GetRootCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryWithSubCategoriesDto?> GetWithSubCategoriesAsync(int id)
        {
            _logger.LogDebug("Getting category with subcategories, ID: {CategoryId}", id);
            var category = await _unitOfWork.Categories.GetWithSubCategoriesAsync(id);

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
                return null;
            }

            return _mapper.Map<CategoryWithSubCategoriesDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            _logger.LogInformation("Creating new category: {CategoryName}", createDto.CategoryName);

            var category = _mapper.Map<Category>(createDto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Category created - CategoryId: {CategoryId}, Name: {CategoryName}",
                category.CategoryId, category.CategoryName);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            _logger.LogDebug("Updating category with ID: {CategoryId}", id);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for update", id);
                return false;
            }

            _mapper.Map(updateDto, category);
            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Category updated - CategoryId: {CategoryId}", id);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            _logger.LogDebug("Deleting category with ID: {CategoryId}", id);

            var hasProducts = await _unitOfWork.Categories.HasProductsAsync(id);
            if (hasProducts)
            {
                _logger.LogWarning("Cannot delete category {CategoryId} - has existing products", id);
                throw new InvalidOperationException("Cannot delete category with existing products");
            }

            await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Category deleted - CategoryId: {CategoryId}", id);
            return true;
        }
    }
}