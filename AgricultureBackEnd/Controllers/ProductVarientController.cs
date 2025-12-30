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
    }
}