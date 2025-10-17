using Orama_App_API.Interfaces;

namespace Orama_App_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendOtpEmailAsync(string email, string otpCode)
        {
            try
            {
                // In a real application, you would integrate with an email service like SendGrid, AWS SES, etc.
                // For now, we'll just log the OTP for development purposes
                _logger.LogInformation($"OTP for {email}: {otpCode}");
                
                // Simulate email sending delay
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send OTP email to {email}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string fullName)
        {
            try
            {
                // In a real application, you would send a welcome email
                _logger.LogInformation($"Welcome email sent to {email} for {fullName}");
                
                // Simulate email sending delay
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send welcome email to {email}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string otpCode)
        {
            try
            {
                // In a real application, you would send a password reset email
                _logger.LogInformation($"Password reset OTP for {email}: {otpCode}");
                
                // Simulate email sending delay
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {email}");
                return false;
            }
        }
    }
}
