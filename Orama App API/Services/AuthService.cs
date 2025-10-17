using Microsoft.IdentityModel.Tokens;
using Orama_App_API.Interfaces;
using Orama_App_API.DTOs;
using Orama_App_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Orama_App_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserService userService,
            IOtpService otpService,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userService = userService;
            _otpService = otpService;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            try
            {
                // Check if email is already registered
                if (await _userService.IsEmailRegisteredAsync(registerDto.Email))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Email is already registered"
                    };
                }

                // Validate password strength
                if (!await _userService.ValidatePasswordStrengthAsync(registerDto.Password))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Password does not meet strength requirements"
                    };
                }

                // Create user
                var user = await _userService.CreateUserAsync(registerDto);

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

                return new RegisterResponseDto
                {
                    Success = true,
                    Message = "User registered successfully",
                    UserId = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                // Get user by email
                var user = await _userService.GetUserByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Verify password
                if (!await _userService.VerifyPasswordAsync(user, loginDto.Password))
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _userService.UpdateUserAsync(user);

                // Generate JWT token
                var token = GenerateJwtToken(user.Id, user.Email);

                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordDto)
        {
            try
            {
                // Check if email is registered
                if (!await _userService.IsEmailRegisteredAsync(forgotPasswordDto.Email))
                {
                    return new ForgotPasswordResponseDto
                    {
                        Success = false,
                        Message = "Email is not registered"
                    };
                }

                // Generate and save OTP
                var otpVerification = await _otpService.CreateOtpVerificationAsync(forgotPasswordDto.Email);

                // Send OTP email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(forgotPasswordDto.Email, otpVerification.OtpCode);

                if (!emailSent)
                {
                    return new ForgotPasswordResponseDto
                    {
                        Success = false,
                        Message = "Failed to send OTP email"
                    };
                }

                return new ForgotPasswordResponseDto
                {
                    Success = true,
                    Message = "OTP sent to your email address"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                return new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "An error occurred during password reset"
                };
            }
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto)
        {
            try
            {
                // Verify OTP
                if (!await _otpService.VerifyOtpAsync(resetPasswordDto.Email, resetPasswordDto.OtpCode))
                {
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired OTP"
                    };
                }

                // Get user
                var user = await _userService.GetUserByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Check if new password is different from current password
                if (await _userService.VerifyPasswordAsync(user, resetPasswordDto.NewPassword))
                {
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "New password must be different from current password"
                    };
                }

                // Check if password was used before
                if (await _userService.IsPasswordUsedBeforeAsync(user.Id, resetPasswordDto.NewPassword))
                {
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "You cannot use a previously used password"
                    };
                }

                // Validate password strength
                if (!await _userService.ValidatePasswordStrengthAsync(resetPasswordDto.NewPassword))
                {
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Password does not meet strength requirements"
                    };
                }

                // Update password
                var passwordUpdated = await _userService.UpdatePasswordAsync(user.Id, resetPasswordDto.NewPassword);

                if (!passwordUpdated)
                {
                    return new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Failed to update password"
                    };
                }

                return new ResetPasswordResponseDto
                {
                    Success = true,
                    Message = "Password reset successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return new ResetPasswordResponseDto
                {
                    Success = false,
                    Message = "An error occurred during password reset"
                };
            }
        }

        public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpRequestDto verifyOtpDto)
        {
            try
            {
                var isValid = await _otpService.VerifyOtpAsync(verifyOtpDto.Email, verifyOtpDto.OtpCode);

                return new VerifyOtpResponseDto
                {
                    Success = true,
                    Message = isValid ? "OTP verified successfully" : "Invalid or expired OTP",
                    IsValid = isValid
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP verification");
                return new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "An error occurred during OTP verification",
                    IsValid = false
                };
            }
        }

        public string GenerateJwtToken(int userId, string email)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "OramaAppAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "OramaAppAPIUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> ValidateJwtTokenAsync(string token)
        {
            try
            {
                var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
                var jwtIssuer = _configuration["Jwt:Issuer"] ?? "OramaAppAPI";
                var jwtAudience = _configuration["Jwt:Audience"] ?? "OramaAppAPIUsers";

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordDto)
        {
            try
            {
                // Get user
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Verify current password
                if (!await _userService.VerifyPasswordAsync(user, changePasswordDto.CurrentPassword))
                {
                    return new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "Current password is incorrect"
                    };
                }

                // Check if new password is different from current password
                if (await _userService.VerifyPasswordAsync(user, changePasswordDto.NewPassword))
                {
                    return new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "New password must be different from current password"
                    };
                }

                // Check if password was used before
                if (await _userService.IsPasswordUsedBeforeAsync(userId, changePasswordDto.NewPassword))
                {
                    return new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "You cannot use a previously used password"
                    };
                }

                // Validate password strength
                if (!await _userService.ValidatePasswordStrengthAsync(changePasswordDto.NewPassword))
                {
                    return new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "Password does not meet strength requirements"
                    };
                }

                // Update password
                var passwordUpdated = await _userService.UpdatePasswordAsync(userId, changePasswordDto.NewPassword);

                if (!passwordUpdated)
                {
                    return new ChangePasswordResponseDto
                    {
                        Success = false,
                        Message = "Failed to update password"
                    };
                }

                return new ChangePasswordResponseDto
                {
                    Success = true,
                    Message = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return new ChangePasswordResponseDto
                {
                    Success = false,
                    Message = "An error occurred during password change"
                };
            }
        }
    }
}
