using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.ProductDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products for category with ID: {CategoryId}", categoryId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string query)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching products with query: {Query}", query);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("featured/{count?}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts(int count)
        {
            try
            {
                var products = await _productService.GetFeaturedProductsAsync(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting featured products");
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
        {
            try
            {
                var newProduct = await _productService.CreateProductAsync(createDto);
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductId }, newProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, updateDto);
                if (updatedProduct == false)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound($"Product with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}