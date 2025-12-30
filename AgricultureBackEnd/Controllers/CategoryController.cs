using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.CategoryDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with ID: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("roots")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetRootCategories()
        {
            try
            {
                var categories = await _categoryService.GetRootCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting root categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/subcategories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetSubcategories(int id)
        {
            try
            {
                var subcategories = await _categoryService.GetWithSubCategoriesAsync(id);
                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting subcategories for category with ID: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            try
            {
                var newCategory = await _categoryService.CreateCategoryAsync(createDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.CategoryId }, newCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new category");
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateDto)
        {
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateDto);
                if (updatedCategory == false)
                {
                    return NotFound($"Category with ID {id} not found");
                }
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound($"Category with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}