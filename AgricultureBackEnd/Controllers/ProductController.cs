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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get products with pagination and filtering
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ProductListDto>>> GetProductsPaged([FromQuery] ProductFilterParams filterParams)
        {
            var result = await _productService.GetProductsPagedAsync(filterParams);
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

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Get products by category with pagination
        /// </summary>
        [HttpGet("category/{categoryId}/paged")]
        public async Task<ActionResult<PagedResult<ProductListDto>>> GetProductsByCategoryPaged(
            int categoryId, 
            [FromQuery] PaginationParams paginationParams)
        {
            var result = await _productService.GetProductsByCategoryPagedAsync(categoryId, paginationParams);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string query)
        {
            var products = await _productService.SearchProductsAsync(query);
            return Ok(products);
        }

        [HttpGet("featured/{count?}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts(int count)
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