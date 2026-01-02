using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgricultureStore.Application.DTOs.AuthDTOs;
using AgricultureStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgricultureBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login user và nhận JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Received login request for username: {Username}", loginDto.UserName);
                
                var response = await _authService.LoginAsync(loginDto);
                
                if (response == null)
                {
                    _logger.LogWarning("Login failed for username: {Username}", loginDto.UserName);
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                _logger.LogInformation("Login successful for username: {Username}", loginDto.UserName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}