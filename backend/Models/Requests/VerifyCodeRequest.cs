using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class VerifyCodeRequest
{
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;
}