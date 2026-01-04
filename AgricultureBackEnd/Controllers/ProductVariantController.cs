using AgricultureStore.Application.DTOs.ProductVariantDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/product-variants")]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetAllVariants()
        {
            var variants = await _productVariantService.GetAllVariantsAsync();
            return Ok(variants.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariantDto>> GetVariantById(int id)
        {
            var variant = await _productVariantService.GetVariantByIdAsync(id);
            if (variant == null)
            {
                return NotFound();
            }
            return Ok(variant);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetByProductId(int productId)
        {
            var variants = await _productVariantService.GetByProductIdAsync(productId);
            return Ok(variants.ToList());
        }

        [HttpGet("lowStock/{threshold}")]
        public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetLowStockVariants(int threshold)
        {
            var variants = await _productVariantService.GetLowStockVariantsAsync(threshold);
            return Ok(variants.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductVariantDto>> CreateVariant([FromBody] CreateProductVariantDto variantDto)
        {
            var createdVariant = await _productVariantService.CreateVariantAsync(variantDto);
            return CreatedAtAction(nameof(GetVariantById), new { id = createdVariant.VariantId }, createdVariant);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductVariantDto>> UpdateVariant(int id, [FromBody] UpdateProductVariantDto variantDto)
        {
            var updatedVariant = await _productVariantService.UpdateVariantAsync(id, variantDto);
            if (!updatedVariant)
            {
                return NotFound();
            }
            return Ok(updatedVariant);
        }

        [HttpPut("updateStock/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVariantStock(int id, [FromBody] int quantity)
        {
            var updated = await _productVariantService.UpdateStockAsync(id, quantity);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var deleted = await _productVariantService.DeleteVariantAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}