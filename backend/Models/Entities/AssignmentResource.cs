using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class AssignmentResource : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public string? FilePath { get; set; } // For downloadable files
    public string? ExternalUrl { get; set; } // For links
    public long? FileSize { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; } = false;
    
    // Foreign Key
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    
    // Helper method
    public string GetSizeDisplay()
    {
        if (Type == ResourceType.Link) return "External Link";
        if (!FileSize.HasValue) return "Unknown";
        
        var size = FileSize.Value;
        if (size < 1024) return $"{size} B";
        if (size < 1024 * 1024) return $"{size / 1024.0:F1} KB";
        if (size < 1024 * 1024 * 1024) return $"{size / (1024.0 * 1024):F1} MB";
        return $"{size / (1024.0 * 1024 * 1024):F1} GB";
    }
}