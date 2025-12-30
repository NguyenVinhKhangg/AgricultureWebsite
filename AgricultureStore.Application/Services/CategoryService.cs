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
            try
            {
                _logger.LogInformation("Getting all categories");
                var categories = await _unitOfWork.Categories.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} categories", categories.Count());
                return _mapper.Map<IEnumerable<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                throw;
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting category with ID: {CategoryId}", id);
                var category = await _unitOfWork.Categories.GetByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved category: {CategoryName}", category.CategoryName);
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category with ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Getting root categories");
                var categories = await _unitOfWork.Categories.GetRootCategoriesAsync();
                _logger.LogInformation("Retrieved {Count} root categories", categories.Count());
                return _mapper.Map<IEnumerable<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root categories");
                throw;
            }
        }

        public async Task<CategoryWithSubCategoriesDto?> GetWithSubCategoriesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting category with subcategories, ID: {CategoryId}", id);
                var category = await _unitOfWork.Categories.GetWithSubCategoriesAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved category: {CategoryName} with {Count} subcategories",
                    category.CategoryName, category.SubCategories.Count);
                return _mapper.Map<CategoryWithSubCategoriesDto>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category with subcategories, ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new category: {CategoryName}", createDto.CategoryName);

                var category = _mapper.Map<Category>(createDto);
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category created successfully: {CategoryName} with ID: {CategoryId}",
                    category.CategoryName, category.CategoryId);
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {CategoryName}", createDto.CategoryName);
                throw;
            }
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating category with ID: {CategoryId}", id);

                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found for update", id);
                    return false;
                }

                _mapper.Map(updateDto, category);
                await _unitOfWork.Categories.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category updated successfully: {CategoryId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting category with ID: {CategoryId}", id);

                var hasProducts = await _unitOfWork.Categories.HasProductsAsync(id);
                if (hasProducts)
                {
                    _logger.LogWarning("Cannot delete category {CategoryId} - has existing products", id);
                    throw new InvalidOperationException("Cannot delete category with existing products");
                }

                await _unitOfWork.Categories.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category deleted successfully: {CategoryId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {CategoryId}", id);
                throw;
            }
        }
    }
}