using AgricultureStore.Application.DTOs.CouponDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Lấy tất cả coupons
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(coupons);
        }

        /// <summary>
        /// Lấy coupon theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CouponDto>> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            
            if (coupon == null)
            {
                return NotFound($"Coupon with ID {id} not found");
            }
            
            return Ok(coupon);
        }

        /// <summary>
        /// Lấy coupon theo code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<CouponDto>> GetCouponByCode(string code)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code);
            
            if (coupon == null)
            {
                return NotFound($"Coupon with code {code} not found");
            }
            
            return Ok(coupon);
        }

        /// <summary>
        /// Lấy danh sách coupons đang active
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetActiveCoupons()
        {
            var coupons = await _couponService.GetActiveCouponsAsync();
            return Ok(coupons);
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
                var coupon = await _couponService.CreateCouponAsync(createDto);
                
                return CreatedAtAction(
                    nameof(GetCouponById), 
                    new { id = coupon.CouponId }, 
                    coupon
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật coupon
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDto updateDto)
        {
            var result = await _couponService.UpdateCouponAsync(id, updateDto);
            
            if (!result)
            {
                return NotFound($"Coupon with ID {id} not found");
            }
            
            return NoContent();
        }

        /// <summary>
        /// Xóa coupon (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            
            if (!result)
            {
                return NotFound($"Coupon with ID {id} not found");
            }
            
            return NoContent();
        }

        /// <summary>
        /// Validate coupon code
        /// </summary>
        [HttpPost("validate")]
        public async Task<ActionResult<bool>> ValidateCoupon([FromBody] ValidateCouponDto validateDto)
        {
            var isValid = await _couponService.ValidateCouponAsync(validateDto.Code);
            
            return Ok(new { 
                isValid = isValid,
                message = isValid ? "Coupon is valid" : "Coupon is invalid or expired"
            });
        }

        /// <summary>
        /// Tính toán discount cho order
        /// </summary>
        [HttpPost("calculate-discount")]
        public async Task<ActionResult<decimal>> CalculateDiscount([FromBody] CalculateDiscountDto calculateDto)
        {
            var discount = await _couponService.CalculateDiscountAsync(
                calculateDto.Code, 
                calculateDto.OrderAmount
            );
            
            return Ok(new { 
                discount = discount,
                finalAmount = calculateDto.OrderAmount - discount
            });
        }
    }
}