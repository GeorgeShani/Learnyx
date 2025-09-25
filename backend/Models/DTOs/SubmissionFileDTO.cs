namespace learnyx.Models.DTOs;

public class SubmissionFileDTO
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public string FileSizeFormatted { get; set; } = string.Empty;
}