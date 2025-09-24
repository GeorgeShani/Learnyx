using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class ResetPasswordRequest
{
    [Required]
    public string ResetToken { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}