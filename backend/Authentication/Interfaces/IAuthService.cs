using learnyx.Models.DTOs;
using learnyx.Models.Requests;
using learnyx.Models.Responses;

namespace learnyx.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Login(LoginRequest request);
    Task<AuthResponse> Register(RegisterRequest request);
    Task<UserDTO?> GetAuthenticatedUserAsync();
}