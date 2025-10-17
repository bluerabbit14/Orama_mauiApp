using Orama_App_API.Models;

namespace Orama_App_API.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync();
        Task<OtpVerification> CreateOtpVerificationAsync(string email);
        Task<OtpVerification?> GetOtpVerificationAsync(string email, string otpCode);
        Task<bool> VerifyOtpAsync(string email, string otpCode);
        Task<bool> InvalidateOtpAsync(string email, string otpCode);
        Task CleanupExpiredOtpsAsync();
    }
}
