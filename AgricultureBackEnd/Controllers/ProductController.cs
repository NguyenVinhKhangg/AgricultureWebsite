using AgricultureStore.Application.DTOs.Common;
using AgricultureStore.Application.DTOs.ProductDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products with optional pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductListDto>>> GetAllProducts([FromQuery] ProductFilterParams? filterParams)
        {
            var result = await _productService.GetAllProductsAsync(filterParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return Ok(product);
        }

        /// <summary>
        /// Get products by category with optional pagination
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<PagedResult<ProductListDto>>> GetProductsByCategory(
            int categoryId, 
            [FromQuery] PaginationParams? paginationParams)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId, paginationParams);
            return Ok(result);
        }

        /// <summary>
        /// Search products with optional pagination
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<ProductListDto>>> SearchProducts(
            [FromQuery] string query,
            [FromQuery] PaginationParams? paginationParams)
        {
            var result = await _productService.SearchProductsAsync(query, paginationParams);
            return Ok(result);
        }

        [HttpGet("featured/{count?}")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetFeaturedProducts(int count = 10)
        {
            var products = await _productService.GetFeaturedProductsAsync(count);
            return Ok(products);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
        {
            var newProduct = await _productService.CreateProductAsync(createDto);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductId }, newProduct);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, updateDto);
            if (updatedProduct == false)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound($"Product with ID {id} not found");
            }
            return NoContent();
        }
    }
}