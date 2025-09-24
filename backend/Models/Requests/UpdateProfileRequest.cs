using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name must not exceed 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name must not exceed 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Bio must not exceed 200 characters.")]
    public string? Bio { get; set; }
}