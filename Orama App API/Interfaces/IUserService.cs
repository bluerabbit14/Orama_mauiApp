using Orama_App_API.Models;
using Orama_App_API.DTOs;

namespace Orama_App_API.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(RegisterRequestDto registerDto);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<bool> VerifyPasswordAsync(User user, string password);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> IsPasswordUsedBeforeAsync(int userId, string password);
        Task SavePasswordToHistoryAsync(int userId, string passwordHash);
        Task<bool> ValidatePasswordStrengthAsync(string password);
        Task<UpdateProfileResponseDto> UpdateUserProfileAsync(int userId, UpdateProfileRequestDto updateProfileDto);
        Task UpdateLastLogoutAsync(int userId);
    }
}
