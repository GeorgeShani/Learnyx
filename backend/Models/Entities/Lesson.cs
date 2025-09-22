using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public LessonType Type { get; set; } = LessonType.Video;
    public string Duration { get; set; } = string.Empty; // e.g., "15:30"
    public string Content { get; set; } = string.Empty; // Video URL, text content, etc.
    public bool IsFree { get; set; } = false;
    public int Order { get; set; }
    public List<string> Resources { get; set; } = new();
    
    // Foreign Key
    public int SectionId { get; set; }
    public CourseSection Section { get; set; } = null!;
    
    // Navigation Properties
    public ICollection<LessonProgress> StudentProgress { get; set; } = new List<LessonProgress>();
}