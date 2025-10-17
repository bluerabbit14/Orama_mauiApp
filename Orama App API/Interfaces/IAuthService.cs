using Orama_App_API.DTOs;

namespace Orama_App_API.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordDto);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto);
        Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpRequestDto verifyOtpDto);
        string GenerateJwtToken(int userId, string email);
        Task<bool> ValidateJwtTokenAsync(string token);
        Task<ChangePasswordResponseDto> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordDto);
    }
}
