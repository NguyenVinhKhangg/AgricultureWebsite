using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.ProductVariantDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductVarientController : ControllerBase
    {
        private readonly IProductVariantService _productVarientService;
        private readonly ILogger<ProductVarientController> _logger;

        public ProductVarientController(IProductVariantService productVarientService, ILogger<ProductVarientController> logger)
        {
            _productVarientService = productVarientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetAllVariants()
        {
            try
            {
                _logger.LogInformation("Received request to get all product variants");
                var variants = await _productVarientService.GetAllVariantsAsync();
                _logger.LogInformation("Returning {Count} product variants", variants.Count());
                return Ok(variants.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all product variants");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariantDto>> GetVariantById(int id)
        {
            try
            {
                _logger.LogInformation("Received request to get product variant with ID {Id}", id);
                var variant = await _productVarientService.GetVariantByIdAsync(id);
                if (variant == null)
                {
                    _logger.LogWarning("Product variant with ID {Id} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Returning product variant with ID {Id}", id);
                return Ok(variant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product variant with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("productVarient/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetByProductId(int productId)
        {
            try
            {
                _logger.LogInformation("Received request to get product variants for product ID {ProductId}", productId);
                var variants = await _productVarientService.GetByProductIdAsync(productId);
                _logger.LogInformation("Returning {Count} product variants for product ID {ProductId}", variants.Count(), productId);
                return Ok(variants.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product variants for product ID {ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("lowStock/{threshold}")]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetLowStockVariants(int threshold)
        {
            try
            {
                _logger.LogInformation("Received request to get product variants with low stock below {Threshold}", threshold);
                var variants = await _productVarientService.GetLowStockVariantsAsync(threshold);
                _logger.LogInformation("Returning {Count} product variants with low stock below {Threshold}", variants.Count(), threshold);
                return Ok(variants.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product variants with low stock below {Threshold}", threshold);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductVariantDto>> CreateVariant([FromBody] CreateProductVariantDto variantDto)
        {
            try
            {
                _logger.LogInformation("Received request to create a new product variant");
                var createdVariant = await _productVarientService.CreateVariantAsync(variantDto);
                _logger.LogInformation("Created new product variant with ID {Id}", createdVariant.VariantId);
                return CreatedAtAction(nameof(GetVariantById), new { id = createdVariant.VariantId }, createdVariant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new product variant");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductVariantDto>> UpdateVariant(int id, [FromBody] UpdateProductVariantDto variantDto)
        {
            try
            {
                _logger.LogInformation("Received request to update product variant with ID {Id}", id);
                var updatedVariant = await _productVarientService.UpdateVariantAsync(id, variantDto);
                if (updatedVariant == null)
                {
                    _logger.LogWarning("Product variant with ID {Id} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Updated product variant with ID {Id}", id);
                return Ok(updatedVariant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product variant with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("updateStock/{id}")]
        public async Task<IActionResult> UpdateVariantStock(int id, [FromBody] int quantity)
        {
            try
            {
                _logger.LogInformation("Received request to update stock for product variant with ID {Id}", id);
                var updated = await _productVarientService.UpdateStockAsync(id, quantity);
                if (!updated)
                {
                    _logger.LogWarning("Product variant with ID {Id} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Updated stock for product variant with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating stock for product variant with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            try
            {
                _logger.LogInformation("Received request to delete product variant with ID {Id}", id);
                var deleted = await _productVarientService.DeleteVariantAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Product variant with ID {Id} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Deleted product variant with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product variant with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}