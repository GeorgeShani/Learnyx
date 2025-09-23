using learnyx.Models.Enums;

namespace learnyx.Models.Requests;

public class LessonRequest
{
    public string Title { get; set; } = string.Empty;
    public LessonType Type { get; set; } = LessonType.Video;
    public string? Duration { get; set; }
    public string? Content { get; set; }
    public bool IsFree { get; set; } = false;
    public int Order { get; set; }
    public List<string> Resources { get; set; } = new();
}