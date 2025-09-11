using learnyx.Models.Entities;
using learnyx.Models.Enums;

namespace learnyx.Authentication.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
    string? GetUserEmailFromToken(string token);
    UserRole? GetUserRoleFromToken(string token);
}