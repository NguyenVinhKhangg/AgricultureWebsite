using AgricultureStore.Application.DTOs.CategoryDTOs;

namespace AgricultureStore.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
        Task<CategoryWithSubCategoriesDto?> GetWithSubCategoriesAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}