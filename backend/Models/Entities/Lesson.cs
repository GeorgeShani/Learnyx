namespace learnyx.Models.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string[]? Attachments { get; set; }
    public int OrderIndex { get; set; }
    
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
}