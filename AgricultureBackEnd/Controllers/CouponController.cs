using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.CouponDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly ILogger<CouponController> _logger;

        public CouponController(ICouponService couponService, ILogger<CouponController> logger)
        {
            _couponService = couponService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả coupons
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons()
        {
            try
            {
                _logger.LogInformation("Received request to get all coupons");
                var coupons = await _couponService.GetAllCouponsAsync();
                _logger.LogInformation("Returning {Count} coupons", coupons.Count());
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all coupons");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy coupon theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CouponDto>> GetCouponById(int id)
        {
            try
            {
                _logger.LogInformation("Received request to get coupon with ID {Id}", id);
                var coupon = await _couponService.GetCouponByIdAsync(id);
                
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon with ID {Id} not found", id);
                    return NotFound($"Coupon with ID {id} not found");
                }
                
                _logger.LogInformation("Returning coupon with ID {Id}", id);
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting coupon with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy coupon theo code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<CouponDto>> GetCouponByCode(string code)
        {
            try
            {
                _logger.LogInformation("Received request to get coupon with code {Code}", code);
                var coupon = await _couponService.GetCouponByCodeAsync(code);
                
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon with code {Code} not found", code);
                    return NotFound($"Coupon with code {code} not found");
                }
                
                _logger.LogInformation("Returning coupon with code {Code}", code);
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting coupon with code {Code}", code);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Lấy danh sách coupons đang active
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetActiveCoupons()
        {
            try
            {
                _logger.LogInformation("Received request to get active coupons");
                var coupons = await _couponService.GetActiveCouponsAsync();
                _logger.LogInformation("Returning {Count} active coupons", coupons.Count());
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active coupons");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tạo coupon mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CouponDto>> CreateCoupon([FromBody] CreateCouponDto createDto)
        {
            try
            {
                _logger.LogInformation("Received request to create new coupon with code {Code}", createDto.Code);
                var coupon = await _couponService.CreateCouponAsync(createDto);
                _logger.LogInformation("Created coupon with ID {CouponId}", coupon.CouponId);
                
                return CreatedAtAction(
                    nameof(GetCouponById), 
                    new { id = coupon.CouponId }, 
                    coupon
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating coupon");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new coupon");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cập nhật coupon
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDto updateDto)
        {
            try
            {
                _logger.LogInformation("Received request to update coupon with ID {Id}", id);
                var result = await _couponService.UpdateCouponAsync(id, updateDto);
                
                if (!result)
                {
                    _logger.LogWarning("Coupon with ID {Id} not found for update", id);
                    return NotFound($"Coupon with ID {id} not found");
                }
                
                _logger.LogInformation("Successfully updated coupon with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating coupon with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Xóa coupon (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try
            {
                _logger.LogInformation("Received request to delete coupon with ID {Id}", id);
                var result = await _couponService.DeleteCouponAsync(id);
                
                if (!result)
                {
                    _logger.LogWarning("Coupon with ID {Id} not found for deletion", id);
                    return NotFound($"Coupon with ID {id} not found");
                }
                
                _logger.LogInformation("Successfully deleted coupon with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting coupon with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Validate coupon code
        /// </summary>
        [HttpPost("validate")]
        public async Task<ActionResult<bool>> ValidateCoupon([FromBody] ValidateCouponDto validateDto)
        {
            try
            {
                _logger.LogInformation("Received request to validate coupon {Code}", validateDto.Code);
                var isValid = await _couponService.ValidateCouponAsync(validateDto.Code);
                _logger.LogInformation("Coupon {Code} validation result: {IsValid}", validateDto.Code, isValid);
                
                return Ok(new { 
                    isValid = isValid,
                    message = isValid ? "Coupon is valid" : "Coupon is invalid or expired"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating coupon {Code}", validateDto.Code);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tính toán discount cho order
        /// </summary>
        [HttpPost("calculate-discount")]
        public async Task<ActionResult<decimal>> CalculateDiscount([FromBody] CalculateDiscountDto calculateDto)
        {
            try
            {
                _logger.LogInformation(
                    "Received request to calculate discount for coupon {Code} with order amount {Amount}", 
                    calculateDto.Code, 
                    calculateDto.OrderAmount
                );
                
                var discount = await _couponService.CalculateDiscountAsync(
                    calculateDto.Code, 
                    calculateDto.OrderAmount
                );
                
                _logger.LogInformation(
                    "Calculated discount: {Discount} for coupon {Code}", 
                    discount, 
                    calculateDto.Code
                );
                
                return Ok(new { 
                    discount = discount,
                    finalAmount = calculateDto.OrderAmount - discount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating discount");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}