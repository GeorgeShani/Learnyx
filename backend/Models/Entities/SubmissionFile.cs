namespace learnyx.Models.Entities;

public class SubmissionFile : BaseEntity
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty; // UUID-based filename
    public string FilePath { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; } // For file ordering
    
    // Foreign Key
    public int SubmissionId { get; set; }
    public Submission Submission { get; set; } = null!;
    
    // Helper method
    public string GetFileSizeFormatted()
    {
        if (FileSize < 1024) return $"{FileSize} B";
        if (FileSize < 1024 * 1024) return $"{FileSize / 1024.0:F1} KB";
        if (FileSize < 1024 * 1024 * 1024) return $"{FileSize / (1024.0 * 1024):F1} MB";
        return $"{FileSize / (1024.0 * 1024 * 1024):F1} GB";
    }
}