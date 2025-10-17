using Microsoft.AspNetCore.Mvc;
using Orama_App_API.Interfaces;

namespace Orama_App_API.Controllers
{
    /// <summary>
    /// Common endpoints that are publicly accessible and don't require authentication
    /// </summary>
    [ApiController]
    [Route("api/common")]
    public class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;

        public CommonController(ILogger<CommonController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>API health status</returns>
        [HttpGet("health")]
        public ActionResult<object> HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        /// <summary>
        /// Get API information and version
        /// </summary>
        /// <returns>API information</returns>
        [HttpGet("info")]
        public ActionResult<object> GetApiInfo()
        {
            return Ok(new
            {
                name = "Orama App API",
                version = "1.0.0",
                description = "Authentication and user management API for Orama App",
                endpoints = new
                {
                    publicAuth = "/api/public/auth",
                    authenticated = "/api/auth",
                    common = "/api/common"
                },
                documentation = "/swagger",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Get server time
        /// </summary>
        /// <returns>Current server time</returns>
        [HttpGet("time")]
        public ActionResult<object> GetServerTime()
        {
            return Ok(new
            {
                utc = DateTime.UtcNow,
                local = DateTime.Now,
                timezone = TimeZoneInfo.Local.Id
            });
        }

        /// <summary>
        /// Check if email is available for registration
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns>Email availability status</returns>
        [HttpGet("check-email")]
        public async Task<ActionResult<object>> CheckEmailAvailability([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (!IsValidEmail(email))
            {
                return BadRequest(new { message = "Invalid email format" });
            }

            try
            {
                // This would typically check against the database
                // For now, we'll return a mock response
                var isAvailable = await Task.FromResult(true); // Mock implementation
                
                return Ok(new
                {
                    email = email,
                    available = isAvailable,
                    message = isAvailable ? "Email is available" : "Email is already registered"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability for {Email}", email);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get password requirements
        /// </summary>
        /// <returns>Password requirements information</returns>
        [HttpGet("password-requirements")]
        public ActionResult<object> GetPasswordRequirements()
        {
            return Ok(new
            {
                requirements = new
                {
                    minLength = 8,
                    maxLength = 100,
                    mustContainUppercase = true,
                    mustContainLowercase = true,
                    mustContainNumber = true,
                    mustContainSpecialCharacter = true,
                    specialCharacters = "@$!%*?&"
                },
                example = "MySecure123!"
            });
        }

        /// <summary>
        /// Validate email format
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>Validation result</returns>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
