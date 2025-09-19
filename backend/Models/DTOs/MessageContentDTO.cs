using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class MessageContentDTO
{
    public MessageContentType ContentType { get; set; }
    public string? TextContent { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int Order { get; set; }
}