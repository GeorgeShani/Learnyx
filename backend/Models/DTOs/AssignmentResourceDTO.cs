using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class AssignmentResourceDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public string? DownloadUrl { get; set; }
    public string? ExternalUrl { get; set; }
    public string SizeDisplay { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }
}