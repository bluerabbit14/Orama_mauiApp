namespace Orama_App_API.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string email, string otpCode);
        Task<bool> SendWelcomeEmailAsync(string email, string fullName);
        Task<bool> SendPasswordResetEmailAsync(string email, string otpCode);
    }
}
