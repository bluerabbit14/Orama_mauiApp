using Microsoft.EntityFrameworkCore;
using Orama_App_API.Data;
using Orama_App_API.Interfaces;
using Orama_App_API.Models;
using Orama_App_API.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace Orama_App_API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.PasswordHistories)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.PasswordHistories)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<User> CreateUserAsync(RegisterRequestDto registerDto)
        {
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email.ToLower(),
                PasswordHash = HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                IsEmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Save password to history
            await SavePasswordToHistoryAsync(user.Id, user.PasswordHash);

            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email.ToLower() && u.IsActive);
        }

        public async Task<bool> VerifyPasswordAsync(User user, string password)
        {
            return VerifyPassword(password, user.PasswordHash);
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.LastLoginAt = DateTime.UtcNow;

            // Save new password to history
            await SavePasswordToHistoryAsync(userId, user.PasswordHash);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsPasswordUsedBeforeAsync(int userId, string password)
        {
            var passwordHash = HashPassword(password);
            return await _context.PasswordHistories
                .AnyAsync(ph => ph.UserId == userId && ph.PasswordHash == passwordHash);
        }

        public async Task SavePasswordToHistoryAsync(int userId, string passwordHash)
        {
            var passwordHistory = new PasswordHistory
            {
                UserId = userId,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordHistories.Add(passwordHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidatePasswordStrengthAsync(string password)
        {
            // Password must be at least 8 characters long
            if (password.Length < 8) return false;

            // Check for at least one uppercase letter
            if (!password.Any(char.IsUpper)) return false;

            // Check for at least one lowercase letter
            if (!password.Any(char.IsLower)) return false;

            // Check for at least one digit
            if (!password.Any(char.IsDigit)) return false;

            // Check for at least one special character
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c))) return false;

            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        public async Task<UpdateProfileResponseDto> UpdateUserProfileAsync(int userId, UpdateProfileRequestDto updateProfileDto)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new UpdateProfileResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateProfileDto.FullName))
                {
                    user.FullName = updateProfileDto.FullName;
                }

                if (updateProfileDto.PhoneNumber != null)
                {
                    user.PhoneNumber = updateProfileDto.PhoneNumber;
                }

                user.UpdatedAt = DateTime.UtcNow;

                var updated = await UpdateUserAsync(user);
                
                if (!updated)
                {
                    return new UpdateProfileResponseDto
                    {
                        Success = false,
                        Message = "Failed to update profile"
                    };
                }

                return new UpdateProfileResponseDto
                {
                    Success = true,
                    Message = "Profile updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new UpdateProfileResponseDto
                {
                    Success = false,
                    Message = "An error occurred while updating profile"
                };
            }
        }

        public async Task UpdateLastLogoutAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user != null)
                {
                    user.LastLogoutAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking logout flow
                // In a real application, you might want to use a logger here
            }
        }
    }
}
