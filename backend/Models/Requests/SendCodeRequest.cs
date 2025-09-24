using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class SendCodeRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}