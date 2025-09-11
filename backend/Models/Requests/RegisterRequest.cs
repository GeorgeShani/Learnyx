using learnyx.Models.Enums;

namespace learnyx.Models.Requests;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public UserRole Role { get; set; }
}