using System.ComponentModel.DataAnnotations;
using learnyx.Models.Enums;

namespace learnyx.Models.Requests;

public class CreateAssignmentResourceRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public ResourceType Type { get; set; }

    [StringLength(500)]
    public string? ExternalUrl { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public int Order { get; set; }

    public bool IsRequired { get; set; } = false;
}