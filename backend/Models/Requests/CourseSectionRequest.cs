namespace learnyx.Models.Requests;

public class CourseSectionRequest
{
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<LessonRequest> Lessons { get; set; } = new();
}