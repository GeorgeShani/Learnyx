using learnyx.Models.DTOs;

namespace learnyx.Models.Responses;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDTO User { get; set; } = null!;
}