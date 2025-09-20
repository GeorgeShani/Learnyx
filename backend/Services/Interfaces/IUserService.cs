using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Enums;

namespace learnyx.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<UserDTO?> GetUserByIdAsync(int id);
    Task<UserDTO?> GetUserByEmailAsync(string email);
    Task<UserDTO?> GetUserByGoogleIdAsync(string googleId);
    Task<UserDTO?> GetUserByFacebookIdAsync(string facebookId);
    Task<UserDTO> CreateUserAsync(User user);
    Task<UserDTO?> UpdateUserAsync(int id, User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UserExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(UserRole role);
    Task<UserDTO?> UpdateUserAvatarAsync(int userId, string avatarUrl);
    Task<bool> DeleteUserAvatarAsync(int userId);
}