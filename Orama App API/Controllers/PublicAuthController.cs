using Microsoft.AspNetCore.Mvc;
using Orama_App_API.Interfaces;
using Orama_App_API.DTOs;

namespace Orama_App_API.Controllers
{
    /// <summary>
    /// Public authentication endpoints that don't require authentication
    /// </summary>
    [ApiController]
    [Route("api/public/auth")]
    public class PublicAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<PublicAuthController> _logger;

        public PublicAuthController(IAuthService authService, ILogger<PublicAuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerDto">User registration details</param>
        /// <returns>Registration result</returns>
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Login user with email and password
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>Login result with JWT token</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Initiate forgot password process by sending OTP to email
        /// </summary>
        /// <param name="forgotPasswordDto">Email address</param>
        /// <returns>OTP sending result</returns>
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponseDto>> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Reset password using OTP and new password
        /// </summary>
        /// <param name="resetPasswordDto">Reset password details</param>
        /// <returns>Password reset result</returns>
        [HttpPost("reset-password")]
        public async Task<ActionResult<ResetPasswordResponseDto>> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Verify OTP code
        /// </summary>
        /// <param name="verifyOtpDto">OTP verification details</param>
        /// <returns>OTP verification result</returns>
        [HttpPost("verify-otp")]
        public async Task<ActionResult<VerifyOtpResponseDto>> VerifyOtp([FromBody] VerifyOtpRequestDto verifyOtpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.VerifyOtpAsync(verifyOtpDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
