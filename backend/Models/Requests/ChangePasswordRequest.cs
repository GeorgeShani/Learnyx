using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "New password must be at least 8 characters long.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}