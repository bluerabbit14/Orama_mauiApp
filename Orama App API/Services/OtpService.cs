using Microsoft.EntityFrameworkCore;
using Orama_App_API.Data;
using Orama_App_API.Interfaces;
using Orama_App_API.Models;

namespace Orama_App_API.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new();

        public OtpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOtpAsync()
        {
            return await Task.FromResult(_random.Next(100000, 999999).ToString());
        }

        public async Task<OtpVerification> CreateOtpVerificationAsync(string email)
        {
            // Invalidate any existing OTPs for this email
            var existingOtps = await _context.OtpVerifications
                .Where(o => o.Email == email.ToLower() && !o.IsUsed)
                .ToListAsync();

            foreach (var otp in existingOtps)
            {
                otp.IsUsed = true;
            }

            var otpCode = await GenerateOtpAsync();
            var otpVerification = new OtpVerification
            {
                Email = email.ToLower(),
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10), // OTP expires in 10 minutes
                IsUsed = false,
                Attempts = 0
            };

            _context.OtpVerifications.Add(otpVerification);
            await _context.SaveChangesAsync();

            return otpVerification;
        }

        public async Task<OtpVerification?> GetOtpVerificationAsync(string email, string otpCode)
        {
            return await _context.OtpVerifications
                .FirstOrDefaultAsync(o => o.Email == email.ToLower() 
                    && o.OtpCode == otpCode 
                    && !o.IsUsed 
                    && o.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<bool> VerifyOtpAsync(string email, string otpCode)
        {
            var otpVerification = await GetOtpVerificationAsync(email, otpCode);
            
            if (otpVerification == null)
            {
                // Increment attempts for failed verification
                var failedOtp = await _context.OtpVerifications
                    .FirstOrDefaultAsync(o => o.Email == email.ToLower() 
                        && o.OtpCode == otpCode 
                        && !o.IsUsed 
                        && o.ExpiresAt > DateTime.UtcNow);
                
                if (failedOtp != null)
                {
                    failedOtp.Attempts++;
                    if (failedOtp.Attempts >= 3) // Block after 3 attempts
                    {
                        failedOtp.IsUsed = true;
                    }
                    await _context.SaveChangesAsync();
                }
                return false;
            }

            // Mark OTP as used
            otpVerification.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> InvalidateOtpAsync(string email, string otpCode)
        {
            var otpVerification = await _context.OtpVerifications
                .FirstOrDefaultAsync(o => o.Email == email.ToLower() && o.OtpCode == otpCode);

            if (otpVerification != null)
            {
                otpVerification.IsUsed = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task CleanupExpiredOtpsAsync()
        {
            var expiredOtps = await _context.OtpVerifications
                .Where(o => o.ExpiresAt < DateTime.UtcNow || o.IsUsed)
                .ToListAsync();

            _context.OtpVerifications.RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
        }
    }
}
